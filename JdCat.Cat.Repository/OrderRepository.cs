using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public new Order Get(int id)
        {
            return Context.Orders
                .Include(a => a.DadaReturn)
                .Include(a => a.Products)
                .SingleOrDefault(a => a.ID == id);
        }
        public OrderRepository(CatDbContext context) : base(context)
        {

        }
        public JsonData CreateOrder(Order order)
        {
            var result = new JsonData();
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
                var nowStr = now.ToString("yyyy-MM-dd");
                var query = Context.Orders.Where(a => a.BusinessId == order.BusinessId && a.CreateTime.Value.ToString("yyyy-MM-dd") == nowStr);
                int max = 0;
                if (query.Count() > 0)
                {
                    max = query.Max(a => a.Identifier);
                }
                order.Identifier = max + 1;
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
            // 清空购物车
            Context.Database.ExecuteSqlCommand("delete dbo.ShoppingCart where userid={0}", order.UserId);
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

        public bool Receive(int orderId)
        {
            var order = new Order { ID = orderId };
            Context.Attach(order);
            order.Status = OrderStatus.Receipted;
            return Context.SaveChanges() > 0;
        }
        public bool Reject(int orderId, string reason)
        {
            var order = new Order { ID = orderId };
            Context.Attach(order);
            order.Status = OrderStatus.Cancel;
            order.RejectReasion = reason;
            return Context.SaveChanges() > 0;
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
                .Include(a => a.SaleFullReduce)
                .Include(a => a.SaleCouponUser).SingleOrDefault(a => a.ID == id);
        }

        public Order PaySuccess(WxPaySuccess ret)
        {
            var order = Context.Orders
            .Include(a => a.Business)
            .Include(a => a.User)
            .Include(a => a.Products)
            .Include(a => a.SaleFullReduce)
            .Include(a => a.SaleCouponUser).SingleOrDefault(a => a.OrderCode == ret.out_trade_no);
            //var order = Context.Orders.Include(a => a.Products).Include(a => a.Business).SingleOrDefault(a => a.OrderCode == ret.out_trade_no);
            if (order == null) return null;
            if (order.Status == OrderStatus.NotPay)
            {
                order.WxPayCode = ret.transaction_id;
                order.PayTime = DateTime.Now;
                order.Status = OrderStatus.Payed;
                order.User.PurchaseTimes++;
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
            else
            {
                return await PrintFeie(order, device, business);
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

        public Order GetOrderByCode(string code)
        {
            return Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).SingleOrDefault(a => a.OrderCode == code);
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
                if (item.Tag1 == null) continue;
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
                    order.Status = OrderStatus.Receipted;
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
                    order.Status = OrderStatus.Receipted;
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
                default:
                    break;
            }
            return null;
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
                default:
                    break;
            }
        }

        public void Print(Business business)
        {
            PrintBalanceTipsAsync("的点点滴滴所多所多", business);
        }

    }
}
