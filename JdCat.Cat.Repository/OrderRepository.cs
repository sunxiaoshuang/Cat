using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Repository.Model;
using JdCat.Cat.Repository.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private static object loop = new object();
        private StackExchange.Redis.IDatabase _database;
        //public new Order Get(int id)
        //{
        //    return Context.Orders
        //        .Include(a => a.DadaReturn)
        //        .Include(a => a.Products)
        //        .SingleOrDefault(a => a.ID == id);
        //}
        public OrderRepository(CatDbContext context, StackExchange.Redis.IConnectionMultiplexer connection) : base(context)
        {
            _database = connection.GetDatabase();
        }
        public JsonData CreateOrder(Order order)
        {
            var result = new JsonData();
            if (order.Products == null || order.Products.Count == 0)
            {
                result.Msg = "系统异常，请返回点餐页重新结算";
                return result;
            }
            if (order.ReceiverAddress.Contains("undefined"))
            {
                result.Msg = "请选择收货地址";
                return result;
            }
            var business = Context.Businesses.Single(a => a.ID == order.BusinessId);
            if (business.IsClose)
            {
                result.Msg = "本店已暂停营业";
                return result;
            }
            var now = DateTime.Now;
            var time1 = IsDoing(business.BusinessStartTime, business.BusinessEndTime);
            var time2 = IsDoing(business.BusinessStartTime2, business.BusinessEndTime2);
            var time3 = IsDoing(business.BusinessStartTime3, business.BusinessEndTime3);
            if (!(time1 || time2 || time3))
            {
                result.Msg = "厨师正在休息，请在营业时间内点单噢！";
                return result;
            }
            lock (loop)
            {
                var orderCode = ExecuteScalar("SELECT CONCAT(DATE_FORMAT(NOW(),'%Y%m%d'), fn_right_padding(NEXT_VAL('OrderNumbers'), 6), fn_right_padding(floor(rand()*100000), 5))") + "";
                order.OrderCode = orderCode;
                Context.Orders.Add(order);
                // 如果使用了优惠券
                if (order.SaleCouponUserId != null)
                {
                    var couponUser = Context.SaleCouponUsers.Include(a => a.Coupon).Single(a => a.ID == order.SaleCouponUserId.Value);
                    couponUser.Status = CouponStatus.Used;
                    couponUser.UseTime = DateTime.Now;
                    couponUser.Coupon.Consumed += 1;
                    if (couponUser.Coupon.Quantity > 0 && couponUser.Coupon.Consumed > couponUser.Coupon.Quantity)
                    {
                        couponUser.Coupon.Consumed = couponUser.Coupon.Quantity;
                    }
                }
                if (Context.SaveChanges() == 0)
                {
                    result.Success = false;
                    result.Msg = "创建订单失败";
                    return result;
                }
            }
            result.Data = order;
            result.Success = true;
            // 清空购物车（以后删除这段代码）
            Context.Database.ExecuteSqlCommand("delete from `ShoppingCart` where userid={0}", order.UserId);
            return result;
        }

        /// <summary>
        /// 是否正在营业
        /// </summary>
        /// <returns></returns>
        private bool IsDoing(string start, string end)
        {
            if (start == null) return true;
            var now = DateTime.Now;
            var startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(start.Split(':')[0]), int.Parse(start.Split(':')[1]), 0);
            var endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(end.Split(':')[0]), int.Parse(end.Split(':')[1]), 0);
            return startTime <= now && endTime >= now;
        }

        public IEnumerable<Order> GetOrder(Business business, OrderStatus? status, PagingQuery query, string code, string phone, int? userId = null, Expression<Func<Order, bool>> expression = null, DateTime? createTime = null)
        {
            var lastTime = DateTime.Now.AddYears(-1);
            var queryable = Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).Where(a => a.BusinessId == business.ID && a.CreateTime > lastTime);
            if (expression != null)
            {
                queryable = queryable.Where(expression);
            }
            if (!string.IsNullOrEmpty(code))
            {
                queryable = queryable.Where(a => a.OrderCode.Contains(code));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                queryable = queryable.Where(a => a.Phone.Contains(phone));
            }
            if (userId != null)
            {
                queryable = queryable.Where(a => a.UserId == userId.Value);
            }
            if (status.HasValue)
            {
                queryable = queryable.Where(a => (a.Status & status) > 0);
            }
            if (createTime.HasValue)
            {
                var flag = createTime.Value.ToString("yyyy-MM-dd");
                queryable = queryable.Where(a => a.CreateTime.Value.ToString("yyyy-MM-dd") == flag);
            }
            query.RecordCount = queryable.Count();
            return queryable.OrderByDescending(a => a.CreateTime).Skip(query.Skip).Take(query.PageSize).ToList();
        }

        public IEnumerable<Order> GetOrder(int businessId, DateTime createTime, PagingQuery query)
        {
            var lastTime = createTime.ToString("yyyy-MM-dd");
            var queryable = Context.Orders
                .Where(a => a.BusinessId == businessId && a.CreateTime.Value.ToString("yyyy-MM-dd") == lastTime && (a.Status & OrderStatus.Valid) > 0)
                .Select(a => new Order { ID = a.ID, Identifier = a.Identifier, OrderCode = a.OrderCode, CreateTime = a.CreateTime, Status = a.Status, Price = a.Price, ReceiverName = a.ReceiverName, Phone = a.Phone });

            query.RecordCount = queryable.Count();
            return queryable.OrderByDescending(a => a.PayTime).Skip(query.Skip).Take(query.PageSize).ToList();
        }

        public Order GetOrderForDetail(int id)
        {
            //var order = Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).FirstOrDefault(a => a.ID == id);
            var order = Context.Orders.Include(a => a.Products)
                .Include(a => a.OrderActivities)
                .FirstOrDefault(a => a.ID == id);
            QuarySetMealProduct(order.Products.Where(a => a.ProductIdSet != null));
            return order;
        }

        public IEnumerable<Order> GetOrders(Business business, OrderStatus? status, PagingQuery query, string code, string phone, int? userId = null, Expression<Func<Order, bool>> expression = null, DateTime? createTime = null)
        {
            var lastTime = DateTime.Now.AddYears(-1);
            var queryable = Context.Orders.Where(a => a.BusinessId == business.ID && a.CreateTime > lastTime);
            if (expression != null)
            {
                queryable = queryable.Where(expression);
            }
            if (!string.IsNullOrEmpty(code))
            {
                queryable = queryable.Where(a => a.OrderCode.Contains(code));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                queryable = queryable.Where(a => a.Phone.Contains(phone));
            }
            if (userId != null)
            {
                queryable = queryable.Where(a => a.UserId == userId.Value);
            }
            if (status.HasValue)
            {
                queryable = queryable.Where(a => (a.Status & status) > 0);
            }
            if (createTime.HasValue)
            {
                var flag = createTime.Value.ToString("yyyy-MM-dd");
                queryable = queryable.Where(a => a.CreateTime.Value.ToString("yyyy-MM-dd") == flag);
            }
            query.RecordCount = queryable.Count();
            return queryable.OrderByDescending(a => a.CreateTime).Skip(query.Skip).Take(query.PageSize).ToList();
        }

        public bool Receive(int orderId)
        {
            var order = new Order { ID = orderId };
            Context.Attach(order);
            order.Status = OrderStatus.Receipted;
            return Context.SaveChanges() > 0;
        }
        public JsonData Reject(int id, string reason, X509Certificate2 cert)
        {
            var result = new JsonData();
            var order = Context.Orders.Include(a => a.Business).SingleOrDefault(a => a.ID == id);
            order.CancelReason = order.RejectReasion = reason;
            try
            {
                // 拒单时，首先执行退款操作
                var refundResult = Refund(order, cert);
                if (refundResult.GetValue("return_code").ToString() != "SUCCESS")
                {
                    result.Msg = refundResult.GetValue("return_msg").ToString();
                    return result;
                }
                if (refundResult.GetValue("err_code_des") + "" != "")
                {
                    result.Msg = refundResult.GetValue("err_code_des").ToString();
                    return result;
                }
                // 再更改订单状态
                order.Status = OrderStatus.Cancel;
                order.RefundStatus = OrderRefundStatus.Finish;
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }
            result.Success = true;
            result.Msg = "操作成功";
            result.Data = order;
            return result;
        }

        public JsonData Cancel(int id, string reason, X509Certificate2 cert)
        {
            var result = new JsonData();
            var order = Context.Orders.Include(a => a.Business).SingleOrDefault(a => a.ID == id);
            order.CancelReason = reason ?? null;
            if (order == null)
            {
                result.Msg = "订单不存在";
                return result;
            }
            try
            {
                // 取消订单时，首先执行退款操作
                var refundResult = Refund(order, cert);
                if (refundResult.GetValue("return_code").ToString() != "SUCCESS")
                {
                    result.Msg = refundResult.GetValue("return_msg").ToString();
                    return result;
                }
                if (refundResult.GetValue("err_code_des") + "" != "")
                {
                    result.Msg = refundResult.GetValue("err_code_des").ToString();
                    return result;
                }
                // 再更改订单状态
                order.Status = OrderStatus.Close;
                order.RefundStatus = OrderRefundStatus.Finish;
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
                return result;
            }
            result.Success = true;
            result.Msg = "操作成功";
            result.Data = order;
            return result;
        }


        public bool CancelSuccess(Order order, DadaResult<DadaLiquidatedDamages> back)
        {
            back.result.Order = order;
            Context.DadaLiquidatedDamageses.Add(back.result);
            order.Status = OrderStatus.CallOff;
            return Context.SaveChanges() > 0;
        }

        public bool SendOrderSelf(int id, Order order = null)
        {
            if (order == null)
            {
                order = new Order { ID = id };
                Context.Attach(order);
            }
            order.Status = OrderStatus.Distribution;
            order.DeliveryMode = DeliveryMode.Own;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            return Context.SaveChanges() > 0;
        }

        public bool Achieve(int id)
        {
            var order = new Order { ID = id };
            Context.Attach(order);
            order.Status = OrderStatus.Achieve;
            order.AchieveTime = DateTime.Now;
            return Context.SaveChanges() > 0;
        }

        public void UpdateOrderStatus(DadaCallBack dada)
        {
            var order = Context.Orders.Include(a => a.DadaCallBacks).SingleOrDefault(a => a.OrderCode == dada.order_id);
            var isDelay = false;                                // 是否是延后的操作
            foreach (var item in order.DadaCallBacks)
            {
                if (item.update_time > dada.update_time)
                {
                    isDelay = true;
                }
            }
            order.DadaCallBacks.Add(dada);
            if (!isDelay)
            {
                // 根据达达返回状态，更新订单的状态
                switch (dada.order_status)
                {
                    case DadaStatus.PendingOrder:
                        order.Status = OrderStatus.DistributorReceipt;
                        break;
                    case DadaStatus.DistributorReceipt:
                        order.Status = OrderStatus.DistributorReceipt;
                        break;
                    case DadaStatus.Distribution:
                        order.Status = OrderStatus.Distribution;
                        order.DistributionTime = DateTime.Now;
                        break;
                    case DadaStatus.Finish:
                        order.Status = OrderStatus.Achieve;
                        order.AchieveTime = DateTime.Now;
                        break;
                    case DadaStatus.Cancel:
                        order.Status = OrderStatus.CallOff;
                        break;
                    case DadaStatus.Expire:
                        order.Status = OrderStatus.CallOff;
                        break;
                    case DadaStatus.AssignmentList:
                        order.Status = OrderStatus.DistributorReceipt;
                        break;
                    case DadaStatus.Returning:
                        order.Status = OrderStatus.Exception;
                        break;
                    case DadaStatus.Returned:
                        order.Status = OrderStatus.CallOff;
                        break;
                    case DadaStatus.Fail:
                        order.Status = OrderStatus.Receipted;
                        break;
                    default:
                        break;
                }
            }
            Context.SaveChanges();
        }

        public void UpdateOrderStatus(YcfkCallback ycfk)
        {
            var orderCode = ycfk.OrderId.Split('_')[0];
            var order = Context.Orders.SingleOrDefault(a => a.OrderCode == orderCode);
            if (order == null) return;
            switch (ycfk.OrderState)
            {
                case 1:             // 已处理
                case 21:            // 等待分配骑手
                case 22:            // 取餐中
                    order.Status = OrderStatus.DistributorReceipt;
                    break;
                case 23:            // 配送中
                    order.Status = OrderStatus.Distribution;
                    break;
                case 100:           // 同意退款
                case 101:           // 拒绝退款
                case 255:           // 已关闭

                    break;
                case 254:           // 已完成
                    order.Status = OrderStatus.Achieve;
                    order.AchieveTime = DateTime.Now;
                    break;
                default:
                    break;
            }
            Context.SaveChanges();
        }

        public IEnumerable<FeyinDevice> GetPrinters(Business business)
        {
            return Context.FeyinDevices.Where(a => a.BusinessId == business.ID).ToList();
        }

        public FeyinDevice GetPrinter(int id)
        {
            return Context.FeyinDevices.FirstOrDefault(a => a.ID == id);
        }

        public Order GetOrderIncludeProduct(int id)
        {
            return Context.Orders
                .Include(a => a.Business)
                .Include(a => a.DadaReturn)
                .Include(a => a.Products)
                .Include(a => a.OrderActivities)
                .Include(a => a.SaleFullReduce)
                .Include(a => a.SaleCouponUser).SingleOrDefault(a => a.ID == id);
        }
        public Order GetOrderOnlyProduct(int id)
        {
            return Context.Orders.Include(a => a.Products).SingleOrDefault(a => a.ID == id);
        }

        public async Task<Order> PaySuccessAsync(WxPaySuccess ret)
        {
            var order = Context.Orders
            .Include(a => a.Business)
            .Include(a => a.User)
            .Include(a => a.Products)
            .Include(a => a.OrderActivities)
            .Include(a => a.SaleFullReduce)
            .Include(a => a.SaleCouponUser).SingleOrDefault(a => a.OrderCode == ret.out_trade_no);

            //var order = Context.Orders.Include(a => a.Products).Include(a => a.Business).SingleOrDefault(a => a.OrderCode == ret.out_trade_no);
            if (order == null) return null;
            if (order.Status == OrderStatus.NotPay)
            {
                var identify = await GetMaxIdentifyAsync(order.BusinessId.Value);
                order.WxPayCode = ret.transaction_id;
                order.PayTime = DateTime.Now;
                order.Status = OrderStatus.Payed;
                order.User.PurchaseTimes++;
                order.Identifier = identify;
                order.Times = order.User.PurchaseTimes;
                Context.SaveChanges();
                return order;
            }
            return null;
        }

        private static readonly Dictionary<int, string> FeyinTokenDic = new Dictionary<int, string>();
        public async Task<string> Print(Order order, FeyinDevice device = null, Business business = null)
        {
            if (device == null)
            {
                device = Context.FeyinDevices.AsNoTracking().FirstOrDefault(a => a.BusinessId == order.BusinessId.Value && a.IsDefault);
                if (device == null) return null;
            }
            if (business == null)
            {
                business = Context.Businesses.AsNoTracking().Single(a => a.ID == order.BusinessId.Value);
            }
            if (device.Type == PrinterType.Feyin)
            {
                return await PrintFeyin(order, device, business);
            }
            else if (device.Type == PrinterType.Yilianyue)
            {
                return await PrintYilianyun(order, device, business);
            }
            else if (device.Type == PrinterType.Feie)
            {
                return await PrintFeie(order, device, business);
            }
            else
            {
                return await PrintWaimaiguanjia(order, device, business);
            }
        }

        /// <summary>
        /// 飞印打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="device"></param>
        /// <param name="business"></param>
        /// <returns></returns>
        private async Task<string> PrintFeyin(Order order, FeyinDevice device, Business business)
        {
            var token = GetFeyinToken(business);
            var printHelper = new FeYinHelper { ApiKey = business.FeyinApiKey, MemberCode = business.FeyinMemberCode, Token = token };
            var ret = await printHelper.PrintAsync(device.Code, order, business);
            if (token != printHelper.Token)
            {
                FeyinTokenDic[business.ID] = printHelper.Token;
            }
            return JsonConvert.SerializeObject(ret);
        }
        private string GetFeyinToken(Business business)
        {
            var token = string.Empty;
            // 记录token
            if (FeyinTokenDic.ContainsKey(business.ID))
            {
                token = FeyinTokenDic[business.ID];
            }
            else
            {
                FeyinTokenDic.Add(business.ID, "");
            }
            return token;
        }

        /// <summary>
        /// 易联云打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="device"></param>
        /// <param name="business"></param>
        /// <returns></returns>
        private async Task<string> PrintYilianyun(Order order, FeyinDevice device, Business business)
        {
            var helper = YlyHelper.GetHelper();
            var res = await helper.PrintAsync(order, device, business);
            return res;
        }

        private async Task<string> PrintFeie(Order order, FeyinDevice device, Business business)
        {
            var helper = FeieHelper.GetHelper();
            var res = await helper.PrintAsync(order, device, business);
            return res;
        }

        private async Task<string> PrintWaimaiguanjia(Order order, FeyinDevice device, Business business)
        {
            var helper = WmgjHelper.GetHelper();
            var res = await helper.PrintAsync(order, device, business);
            return res;
        }

        public Order GetOrderByCode(string code)
        {
            return Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).SingleOrDefault(a => a.OrderCode == code);
        }
        public int GetOrderIdByCode(string orderCode)
        {
            var ids = Context.Orders.Where(a => a.OrderCode == orderCode).Select(a => a.ID).ToList();
            return ids[0];
        }

        public void QuarySetMealProduct(IEnumerable<OrderProduct> products)
        {
            var arr = new List<int>();
            foreach (var item in products)
            {
                var ids = item.ProductIdSet.Split(',').Select(b => int.Parse(b));
                if (ids.Count() == 0) continue;
                foreach (var id in ids)
                {
                    if (arr.Contains(id)) continue;
                    arr.Add(id);
                }
                item.Tag1 = ids;
            }
            var productList = Context.Products.AsNoTracking().Where(a => arr.Contains(a.ID));
            foreach (var item in products)
            {
                if (item.Tag1 == null)
                {
                    item.Tag1 = "";
                    continue;
                }
                var productArr = new List<Product>();
                foreach (var id in (IEnumerable<int>)item.Tag1)
                {
                    var product = productList.FirstOrDefault(b => b.ID == id);
                    if (product == null) continue;
                    productArr.Add(product);
                }
                item.Tag1 = productArr;
            }
        }

        public DWDStore GetDwdShop(int businessId)
        {
            return Context.DWDStores.AsNoTracking().FirstOrDefault(a => a.BusinessId == businessId);
        }

        public Order UpdateOrderByDwd(DWD_Callback callback, double cost)
        {
            var orderCode = callback.order_original_id.Split('_')[0];
            var order = Context.Orders.SingleOrDefault(a => a.OrderCode == orderCode);
            order.CallbackCost = cost;
            // 根据达达返回状态，更新订单的状态
            switch (callback.order_status)
            {
                case DWD_OrderStatus.Assigning:
                    break;
                case DWD_OrderStatus.Transfer:
                    //order.Status = OrderStatus.Receipted;
                    break;
                case DWD_OrderStatus.Taking:
                    order.Status = OrderStatus.DistributorReceipt;
                    break;
                case DWD_OrderStatus.ArrivedShop:
                    order.Status = OrderStatus.DistributorReceipt;
                    break;
                case DWD_OrderStatus.Distribution:
                    order.DistributionTime = DateTime.Now;
                    order.Status = OrderStatus.Distribution;
                    break;
                case DWD_OrderStatus.Finish:
                    order.Status = OrderStatus.Achieve;
                    order.AchieveTime = DateTime.Now;
                    break;
                case DWD_OrderStatus.Exception:
                    order.Status = OrderStatus.Payed;
                    order.ErrorReason = callback.abnormal_reason;
                    break;
                case DWD_OrderStatus.Cancel:
                    order.Status = OrderStatus.CallOff;
                    order.ErrorReason = callback.cancel_reason;
                    break;
                default:
                    break;
            }
            Context.SaveChanges();
            return order;
        }

        public async Task AutoReceipt(Order order)
        {
            if (!order.Business.IsAutoReceipt) return;

            if (order.Business.ServiceProvider == LogisticsType.None)
            {
                order.Status = OrderStatus.Receipted;
            }
            else
            {
                order.LogisticsType = order.Business.ServiceProvider;

                // 配送订单
                await Invoice(order);
            }
            Context.SaveChanges();
        }

        public async Task<JsonData> Invoice(Order order)
        {
            switch (order.LogisticsType)
            {
                case LogisticsType.Dada:
                    return await DadaHandler(order);
                case LogisticsType.Dianwoda:
                    return await DwdHandler(order);
                case LogisticsType.Fengniao:
                    return null;
                case LogisticsType.Meituan:
                    return null;
                case LogisticsType.Self:
                    return SelfHandler(order);
                case LogisticsType.Yichengfeike:
                    return await YcfkHandler(order);
                case LogisticsType.Shunfeng:
                    return await SfHandler(order);
                default:
                    break;
            }
            return null;
        }

        public JsonData ApplyRefund(int id, string reason)
        {
            var result = new JsonData();
            var order = Get(id);
            if ((order.Status & OrderStatus.CanRefund) == 0)
            {
                result.Msg = "当前状态不允许退款";
                return result;
            }
            order.RefundStatus = OrderRefundStatus.Apply;
            order.RefundReason = reason;
            order.RefundTime = DateTime.Now;
            Context.SaveChanges();
            result.Success = true;
            result.Msg = "申请成功";
            result.Data = order;
            return result;
        }

        public async Task<JsonData> EstimateFreight(int businessId, double lat, double lng, string address)
        {
            var result = new JsonData { Success = true };
            var store = Context.DWDStores.FirstOrDefault(a => a.BusinessId == businessId);
            if (store != null)
            {
                var helper = DwdHelper.GetHelper();
                var ret = await helper.CostAsync(lng, lat, address, store);
                Log.Debug(ret);
                if (ret.errorCode == "0")
                {
                    result.Data = ret.result.total;
                    return result;
                }
            }
            // 如果没有创建点我达商户或者点我达预估接口异常，则返回商户设置的运费
            result.Data = Context.Businesses.Single(a => a.ID == businessId).Freight.Value * 100;
            return result;
        }

        /// <summary>
        /// 达达配送
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<JsonData> DadaHandler(Order order)
        {
            var result = new JsonData();
            var helper = DadaHelper.GetHelper();
            DadaResult<DadaReturn> back;
            try
            {
                back = await helper.SendOrderAsync(order, order.Business);
            }
            catch (Exception e)
            {
                Log.Debug("达达配送出错", e);
                throw;
            }
            // 发送订单未成功
            if (!back.IsSuccess())
            {
                result.Msg = back.msg;
                order.ErrorReason = back.msg;
                return result;
            }
            back.result.Order = order;
            if (order.DadaReturn != null)
            {
                Context.DadaReturns.Remove(order.DadaReturn);
            }
            Context.DadaReturns.Add(back.result);
            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            order.IsSendDada = true;
            result.Success = true;
            result.Msg = "配送成功";
            result.Data = new
            {
                Mode = DeliveryMode.Third,
                Logistics = LogisticsType.Dada,
                order.Status
            };
            return result;
        }

        /// <summary>
        /// 点我达配送
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<JsonData> DwdHandler(Order order)
        {
            var result = new JsonData();
            var helper = DwdHelper.GetHelper();
            var shop = Context.DWDStores.SingleOrDefault(a => a.BusinessId == order.BusinessId);
            if (shop == null)
            {
                result.Msg = "尚未创建点我达商户，请进入[第三方外卖管理->点我达设置]完成初始化操作";
                return result;
            }
            order.Business.DWDStore = shop;
            var back = await helper.SendOrderAsync(order, order.Business);

            // 每次发单后，均检查点我达余额，如果不足20元，则通过打印机提示用户
            var balance = await helper.GetBalanceAsync(shop.external_shopid);
            if (balance.errorCode == "0")
            {
                if (balance.result.balance < 2000)
                {
                    PrintBalanceTipsAsync($"您的点我达账户余额为：{((double)balance.result.balance) / 100}，请及时充值，余额不足时，配送服务将不可用", order.Business);
                }
            }

            // 发送订单未成功
            if (!back.success)
            {
                if (back.message == "服务不可用")
                {
                    back.message = "账户余额不足，请充值，充值方法：第三方外卖管理->点我达设置->充值";
                }
                result.Msg = back.message;
                order.ErrorReason = back.message;
                return result;
            }
            // 订单发送成功后，调用接口获取当前订单的配送费，并写入订单信息中
            var priceResult = await helper.GetOrderPriceAsync(helper.GetOrderCode(order));
            if (priceResult.success)
            {
                order.CallbackCost = priceResult.result.receivable_price / 100;
            }
            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            result.Success = true;
            result.Msg = "配送成功";
            result.Data = new
            {
                Mode = order.DeliveryMode,
                Logistics = order.LogisticsType,
                order.Status,
                flow = order.DistributionFlow
            };
            return result;
        }

        /// <summary>
        /// 一城飞客配送
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<JsonData> YcfkHandler(Order order)
        {
            var result = new JsonData();
            var helper = YcfkHelper.GetHelper();
            order.DistributionFlow++;       // 每次发送订单前，配送流水均增加1
            var ycfkOrder = new YcfkOrder
            {
                OrderId = order.OrderCode + "_" + order.DistributionFlow,
                ViewOrderId = order.OrderCode,
                ShopId = order.Business.StoreId,
                ShopName = order.Business.Name,
                OrderUserName = order.ReceiverName,
                OrderUserPhone = order.Phone,
                OrderUserAddress = order.ReceiverAddress,
                OrderRemark = order.Remark,
                BoxFee = Convert.ToDecimal(order.PackagePrice),
                Freight = Convert.ToDecimal(order.Freight),
                ActivityMoney = Convert.ToDecimal(order.SaleCouponUserMoney ?? 0 + order.SaleFullReduceMoney ?? 0),
                UserGaodeCoordinate = order.Lng + "|" + order.Lat,
                DayIndex = order.Identifier,
                Flag = "简单猫"
            };

            ycfkOrder.FoodList = order.Products.Select(a => new YcfkFoodItem
            {
                FoodName = a.Name,
                FoodPrice = Convert.ToDecimal(a.Price / a.Quantity),
                FoodCount = Convert.ToInt32(a.Quantity)
            }).ToList();

            var json = await helper.Send(ycfkOrder, order.Business.YcfkKey, order.Business.YcfkSecret);
            try
            {
                var jObj = JObject.Parse(json);
                var code = jObj["StateCode"].Value<int>();
                if (code > 0)
                {
                    result.Msg = jObj["StateMsg"].Value<string>();
                    order.ErrorReason = result.Msg;
                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception("一城飞客自动发单异常：" + e.Message + $"。返回值：【{json}】");
            }

            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            result.Success = true;
            result.Msg = "配送成功";
            result.Data = new
            {
                Mode = order.DeliveryMode,
                Logistics = order.LogisticsType,
                order.Status,
                flow = order.DistributionFlow
            };
            return result;
        }

        private async Task<JsonData> SfHandler(Order order)
        {
            var business = order.Business;
            var isProduct = AppSetting.AppData.RunMode == "product";
            var sf = new SfInputData(business.ShunfengDevId, business.ShunfengDevKey);
            var orderCode = order.DistributionFlow == 0 ? order.OrderCode : $"{order.OrderCode}_{order.DistributionFlow}";
            sf.SetValue("dev_id", business.ShunfengDevId);
            sf.SetValue("shop_id", business.ShunfengShopId);
            sf.SetValue("shop_order_id", orderCode);
            sf.SetValue("order_source", "简单猫");
            sf.SetValue("order_sequence", order.Identifier.ToString());
            sf.SetValue("pay_type", 1);
            sf.SetValue("order_time", order.PayTime.Value.ToTimestamp());
            sf.SetValue("is_appoint", 0);
            sf.SetValue("is_insured", 0);
            sf.SetValue("push_time", DateTime.Now.ToTimestamp());
            sf.SetValue("version", 17);
            sf.SetValue("receive", new
            {
                user_name = isProduct ? order.ReceiverName : "顺丰同城",
                user_phone = isProduct ? order.Phone : "18888888888",
                user_address = isProduct ? order.ReceiverAddress : "北京市海淀区学清嘉创大厦A座15层",
                user_lng = isProduct ? order.Lng.ToString() : "116.3534196",
                user_lat = isProduct ? order.Lat.ToString() : "40.0159778"
            });
            sf.SetValue("order_detail", new
            {
                total_price = order.Price * 100,
                product_type = 1,
                weight_gram = 0,
                product_num = 1,
                product_type_num = 1,
                product_detail = order.Products.Select(a => new
                {
                    product_name = a.Name,
                    product_num = a.Quantity
                })

            });
            var sign = sf.MakeSign();

            var url = $"{AppSetting.AppData.ShunfengHost}/open/api/external/createorder?sign={sign}";
            var res = await UtilHelper.RequestAsync(url, sf.ToBody());

            var result = new JsonData();
            var json = JObject.Parse(res);
            if (json["error_code"].Value<int>() != 0)
            {
                result.Msg = json["error_msg"].Value<string>();
                order.ErrorReason = result.Msg;
                return result;
            }

            order.DistributionFlow++;       // 每次发单成功之后，配送流水号加一
            order.DistributionId = json["result"]["sf_order_id"].Value<string>(); // 记录配送订单号
            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            result.Success = true;
            result.Msg = "配送成功";
            result.Data = new
            {
                Mode = order.DeliveryMode,
                Logistics = order.LogisticsType,
                order.Status,
                flow = order.DistributionFlow
            };
            return result;
        }

        public YcfkLocation GetOrderLocation(int id)
        {
            return Context.YcfkLocations.OrderByDescending(a => a.ID).FirstOrDefault(a => a.OrderId == id);
        }

        public JsonData Comment(OrderComment comment)
        {
            comment.IsShow = true;
            if (comment.OrderProducts != null && comment.OrderProducts.Count > 0)
            {
                var products = Context.OrderProducts.Where(a => a.OrderId == comment.OrderId).ToList();
                comment.OrderProducts.ForEach(a =>
                {
                    var product = products.FirstOrDefault(b => a.ID == b.ID);
                    if (product == null) return;
                    product.CommentResult = a.CommentResult;
                    product.OrderComment = comment;
                });
            }
            Context.OrderComments.Add(comment);
            var order = new Order { ID = comment.OrderId };
            Context.Attach(order);
            order.Status = OrderStatus.Appraised;
            return new JsonData
            {
                Success = Context.SaveChanges() > 0
            };
        }

        public List<OrderComment> GetComments(CommentLevel deliveryLevel, CommentLevel orderLevel, DateTime start, DateTime end, PagingQuery paging, params int[] businessIds)
        {
            var endDate = end.AddDays(1);
            var query = Context.OrderComments
                .Include(a => a.Business)
                .Where(a => a.CreateTime >= start && a.CreateTime <= endDate && businessIds.Contains(a.BusinessId));
            if (deliveryLevel != 0)
            {
                query = query.Where(a => (a.DeliveryScore & deliveryLevel) > 0);
            }
            if (orderLevel != 0)
            {
                query = query.Where(a => (a.OrderScore & orderLevel) > 0);
            }
            query = query.OrderByDescending(a => a.CreateTime);
            paging.RecordCount = query.Count();
            return query.Skip(paging.Skip).Take(paging.PageSize).ToList();
        }

        public void ChangeCommentVisible(int commentId, bool visible)
        {
            var comment = Context.OrderComments.FirstOrDefault(a => a.ID == commentId);
            if (comment == null) return;
            comment.IsShow = visible;
            Context.SaveChanges();
            ReloadCommentScore(comment.BusinessId);
        }

        public void ReloadCommentScore(int id)
        {

            var business = Context.Businesses.FirstOrDefault(a => a.ID == id);
            if (business == null) return;
            var query = Context.OrderComments.Where(a => a.BusinessId == id && a.IsShow);

            var orderScoreGroup = query.GroupBy(a => a.OrderScore)
                .Select(a => new Tuple<CommentLevel, int>(a.Key, a.Count()))
                .ToList();
            business.Score = Avg(orderScoreGroup);

            var deliveryScoreGroup = query.GroupBy(a => a.DeliveryScore)
                .Select(a => new Tuple<CommentLevel, int>(a.Key, a.Count()))
                .ToList();
            business.Delivery = Avg(deliveryScoreGroup);
            Context.SaveChanges();
        }

        public async Task<bool> AddOrderNotifyAsync(Order order)
        {
            order.User = null;
            order.Business = null;
            var timespan = new TimeSpan(2, 0, 0);
            return await _database.StringSetAsync($"Jiandanmao:Notify:Order:{order.BusinessId}:{order.ID}", JsonConvert.SerializeObject(order, AppData.JsonSetting), timespan);
        }


        //public async Task<List<Order>> GetOrdersIncludeProductAsync(DateTime? endDate = null)
        //{
        //    var time = endDate ?? DateTime.Now;
        //    //var time = new DateTime(2019, 8, 27);
        //    //var start = new DateTime(2019, 8, 20);
        //    return await Context.Orders.Include(a => a.Products).Where(a => a.CreateTime.Value < time).ToListAsync();
        //}

        //public async Task<List<Order>> GetOrdersNotActivityAsync()
        //{
        //    var orders = ExecuteReader<Order>(@"select ID, OrderCode from (
	       //                             select a.*, b.ID as BiD from `order` a 
		      //                              left join orderactivity b on a.id=b.OrderId
	       //                             where SaleFullReduceId is not null or SaleCouponUserId is not null
        //                            )t where BiD is null");
        //    var ids = orders.Select(a => a.ID).ToList();
        //    return await Context.Orders.Include(a => a.Products).Where(a => ids.Contains(a.ID)).ToListAsync();
        //}








        /// <summary>
        /// 自己配送
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private JsonData SelfHandler(Order order)
        {
            order.Status = OrderStatus.Distribution;
            order.DeliveryMode = DeliveryMode.Own;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            return new JsonData
            {
                Msg = "配送成功",
                Success = true,
                Data = new
                {
                    Mode = DeliveryMode.Own,
                    Status = OrderStatus.Distribution,
                }
            };
        }

        /// <summary>
        /// 打印配送服务余额不足的提示
        /// </summary>
        /// <param name="business"></param>
        private async void PrintBalanceTipsAsync(string content, Business business)
        {
            var device = Context.FeyinDevices.FirstOrDefault(a => a.BusinessId == business.ID && a.IsDefault);
            if (device == null) return;
            switch (device.Type)
            {
                case PrinterType.Feyin:
                    var feyin = new FeYinHelper() { ApiKey = business.FeyinApiKey, MemberCode = business.FeyinMemberCode, Token = GetFeyinToken(business) };
                    await feyin.PrintAsync(content, device.Code);
                    break;
                case PrinterType.Yilianyue:
                    var yly = YlyHelper.GetHelper();
                    await yly.PrintAsync(content, device);
                    break;
                case PrinterType.Feie:
                    var feie = FeieHelper.GetHelper();
                    await feie.PrintAsync(content, device);
                    break;
                case PrinterType.Waimaiguanjia:
                    var wmgj = WmgjHelper.GetHelper();
                    await wmgj.PrintAsync(content, device);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 订单退款
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private InputData Refund(Order order, X509Certificate2 cert)
        {
            var url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            order.RefundNo = Guid.NewGuid().ToString().ToLower();
            var data = new InputData(order.Business.PayServerKey);
            var price = (int)Math.Round(order.Price.Value * 100, 0);        // 订单金额
            if (order.BusinessId == 1) price = 1;

            data.SetValue("appid", order.Business.PayServerAppId);
            data.SetValue("mch_id", order.Business.PayServerMchId);
            data.SetValue("sub_appid", order.Business.AppId);
            data.SetValue("sub_mch_id", order.Business.MchId);
            data.SetValue("transaction_id", order.WxPayCode);
            data.SetValue("out_trade_no", order.OrderCode);
            data.SetValue("total_fee", price);
            data.SetValue("refund_fee", price);
            data.SetValue("out_refund_no", order.RefundNo);
            data.SetValue("refund_desc", order.RefundReason ?? order.CancelReason);
            data.SetValue("nonce_str", Guid.NewGuid().ToString().Substring(0, 30));
            data.SetValue("sign_type", "MD5");
            data.SetValue("sign", data.MakeSign());
            var xml = data.ToXml();


            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            ServicePointManager.DefaultConnectionLimit = 200;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = string.Format("JdCat/{3} ({0}) .net/{1} {2}", Environment.OSVersion, Environment.Version, 1509509871, "1.0.0");
            request.Method = "POST";
            request.Timeout = 2 * 1000;

            request.ContentType = "text/xml";
            byte[] buffer = Encoding.UTF8.GetBytes(xml);
            request.ContentLength = buffer.Length;

            request.ClientCertificates.Add(cert);

            reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            response = (HttpWebResponse)request.GetResponse();

            var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            result = sr.ReadToEnd().Trim();
            sr.Close();
            response.Close();

            Log.Debug("退款返回值：" + result);

            var ret = new InputData(order.Business.PayServerKey);
            ret.FromXml(result);

            return ret;
        }

        /// <summary>
        /// 给绑定的用户发送通知
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="businessId"></param>
        //private async Task SendMessageNotify(WxEventMessage msg, int businessId)
        //{
        //    if (string.IsNullOrEmpty(msg.template_id)) return;
        //    var users = Context.WxListenUsers.Where(a => a.BusinessId == businessId);
        //    if (users == null || users.Count() == 0) return;
        //    foreach (var item in users)
        //    {
        //        msg.touser = item.openid;
        //        var result = await WxHelper.SendEventMessageAsync(msg);
        //        var ret = JsonConvert.DeserializeObject<WxMessageReturn>(result);
        //        if (ret.errcode != 0)
        //        {
        //            Log.Info($"模版通知消息失败：[消息_{JsonConvert.SerializeObject(msg)}，结果_{result}]");
        //        }
        //    }
        //}

        /// <summary>
        /// 获取今日最大订单流水
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetMaxIdentifyAsync(int businessId)
        {
            //var max = 0;
            //var now = DateTime.Now.ToString("yyyy-MM-dd");
            //var query = Context.Orders.Where(a => a.BusinessId == businessId && a.CreateTime.Value.ToString("yyyy-MM-dd") == now);
            //if (query.Count() > 0)
            //{
            //    max = query.Max(a => a.Identifier);
            //}
            //return max;

            var now = DateTime.Now.ToString("yyyy-MM-dd");
            var key = $"Jiandanmao:Util:OrderDayNo:{businessId}:{now}";
            var result = await _database.StringIncrementAsync(key);
            return (int)result;
        }

        /// <summary>
        /// 计算平均得分
        /// </summary>
        /// <param name="tuples"></param>
        /// <returns></returns>
        private double Avg(IEnumerable<Tuple<CommentLevel, int>> tuples)
        {
            if (tuples.Count() == 0) return 5;
            var allCount = 0;
            var allScore = 0d;
            foreach (var item in tuples)
            {
                var score = 0;
                switch (item.Item1)
                {
                    case CommentLevel.None:
                        break;
                    case CommentLevel.Bad:
                        score = 1;
                        break;
                    case CommentLevel.Commonly:
                        score = 2;
                        break;
                    case CommentLevel.Normal:
                        score = 3;
                        break;
                    case CommentLevel.VerySatisfied:
                        score = 4;
                        break;
                    case CommentLevel.Perfect:
                        score = 5;
                        break;
                    default:
                        break;
                }
                if (score == 0) break;
                allCount += item.Item2;
                allScore += item.Item2 * score;
            }
            var avg = Math.Round(allScore / allCount, 1);
            return avg;
        }


    }
}
