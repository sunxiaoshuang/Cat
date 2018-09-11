﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
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
        public Order Get(int id)
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
            var business = Context.Businesses.Single(a => a.ID == order.BusinessId);
            if (business.IsClose)
            {
                result.Msg = "本店已暂停营业";
                return result;
            }
            var now = DateTime.Now;
            var startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(business.BusinessStartTime.Split(':')[0]), int.Parse(business.BusinessStartTime.Split(':')[1]), 0);
            var endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(business.BusinessEndTime.Split(':')[0]), int.Parse(business.BusinessEndTime.Split(':')[1]), 0);
            if (startTime > now || endTime < now)
            {
                result.Msg = $"本店营业时间：{business.BusinessStartTime}-{business.BusinessEndTime}";
                return result;
            }
            lock (loop)
            {
                var nowStr = now.ToString("yyyy-MM-dd");
                var query = Context.Orders.Where(a => a.BusinessId == order.BusinessId && a.CreateTime.Value.ToString("yyyy-MM-dd") == nowStr);
                int max = 0;
                if(query.Count() > 0)
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

        public bool SendSuccess(Order order, DadaResult<DadaReturn> back)
        {
            back.result.Order = order;
            if (order.DadaReturn != null)
            {
                Context.DadaReturns.Remove(order.DadaReturn);
            }
            Context.DadaReturns.Add(back.result);
            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.LogisticsType = LogisticsType.Dada;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
            order.IsSendDada = true;
            return Context.SaveChanges() > 0;
        }
        public bool SendDwdSuccess(Order order, DWD_Result<DWD_Content> back)
        {
            order.Status = OrderStatus.DistributorReceipt;
            order.DeliveryMode = DeliveryMode.Third;
            order.LogisticsType = LogisticsType.Dianwoda;
            order.DistributionTime = DateTime.Now;
            order.ErrorReason = "";
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

        public Order GetOrderIncludeProduct(int id)
        {
            return Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).SingleOrDefault(a => a.ID == id);
        }

        public Order PaySuccess(WxPaySuccess ret)
        {
            var order = Context.Orders.Single(a => a.OrderCode == ret.out_trade_no);
            if (order.Status == OrderStatus.NotPay)
            {
                order.WxPayCode = ret.transaction_id;
                order.PayTime = DateTime.Now;
                order.Status = OrderStatus.Payed;
                //var business = Context.Businesses.Single(a => a.ID == order.BusinessId);
                //if (business.IsAutoReceipt)
                //{
                //    order.Status = OrderStatus.Receipted;
                //}
                Context.SaveChanges();
                return order;
            }
            return null;
        }

        public Order GetOrderByCode(string code)
        {
            return Context.Orders.Include(a => a.Products).Include(a => a.SaleFullReduce).Include(a => a.SaleCouponUser).SingleOrDefault(a => a.OrderCode == code);
        }

        public DWD_Business GetDwdShop(int businessId)
        {
            return Context.DWD_Businesses.AsNoTracking().FirstOrDefault(a => a.BusinessId == businessId);
        }

    }
}
