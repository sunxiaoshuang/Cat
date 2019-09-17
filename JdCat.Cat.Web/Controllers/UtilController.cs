using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    /// <summary>
    /// 通用控制器
    /// </summary>
    public class UtilController : BaseController<IUtilRepository, Business>
    {
        public UtilController(AppData appData, IUtilRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 当系统的数据需要更新时，执行该方法
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Init([FromServices]IOrderRepository servcie, [FromServices]AppData appData)
        {
            ////// 第一步
            ////var orders = await servcie.GetOrdersIncludeProductAsync(new DateTime(2019, 9, 3));

            //// 第二步
            //var orders = await servcie.GetOrdersNotActivityAsync();

            //var activities = new List<OrderActivity>();
            //foreach (var order in orders)
            //{
            //    if (activities.Count > 1000)
            //    {
            //        await servcie.AddRangeAsync(activities);
            //        activities.Clear();
            //    }
            //    if (order.SaleFullReduceId != null)
            //    {
            //        activities.Add(new OrderActivity { ActivityId = order.SaleFullReduceId.Value, Amount = order.SaleFullReduceMoney.Value, OrderId = order.ID, Remark = $"满减优惠{order.SaleFullReduceMoney}元", Type = OrderActivityType.FullReduce });
            //    }
            //    if (order.SaleCouponUserId != null)
            //    {
            //        activities.Add(new OrderActivity { ActivityId = order.SaleCouponUserId.Value, Amount = order.SaleCouponUserMoney.Value, OrderId = order.ID, Remark = $"{order.SaleFullReduceMoney}元优惠券", Type = OrderActivityType.Coupon });
            //    }
            //    if (order.Products == null || order.Products.Count == 0) continue;
            //    foreach (var product in order.Products)
            //    {
            //        if (product.SaleProductDiscountId == null || product.SaleProductDiscountId.Value == 0) continue;
            //        var discountAmount = product.OldPrice - product.Price;
            //        if (discountAmount == null || discountAmount <= 0) continue;
            //        discountAmount = Math.Round(discountAmount.Value, 2);
            //        activities.Add(new OrderActivity { ActivityId = product.SaleProductDiscountId.Value, Amount = discountAmount.Value, OrderId = order.ID, Remark = $"{product.Name}折扣优惠{discountAmount}元", Type = OrderActivityType.ProductDiscount });
            //    }
            //}
            //await servcie.AddRangeAsync(activities);



            return Ok("同步订单活动完成");
        }

    }
}
