using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JdCat.Cat.MpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSameDomain")]
    public class ManaController : BaseController<IMpRepository, Business>
    {
        public ManaController(IMpRepository service) : base(service)
        {
        }

        /// <summary>
        /// 根据用户的openid获取用户已绑定的商户
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [HttpGet("business")]
        public async Task<IActionResult> GetBusinesses([FromQuery]string openid)
        {
            var businesses = await Service.GetBusinessForOpenIdAsync(openid);
            if (businesses == null) return NoContent();
            return Json(businesses.Select(a => new { a.Name, a.ID }));
        }

        /// <summary>
        /// 根据商户id获取商户对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getstore/{id}")]
        public async Task<IActionResult> GetBusiness(int id)
        {
            return Json(await Service.GetAsync<Business>(id));
        }

        [HttpPut("business")]
        public async Task<IActionResult> UpdateBusiness([FromQuery]string field, [FromBody]Business business)
        {
            await Service.UpdateAsync(business, new[] { field });
            return Json(new JsonData { Success = true, Msg = "修改成功" });
        }

        [HttpPut("city")]
        public async Task<IActionResult> UpdateCity([FromBody]Business business)
        {
            await Service.UpdateAsync(business, new[] { "Province", "City", "Area" });
            return Json(new JsonData { Success = true, Msg = "修改成功" });
        }

        [HttpPut("time")]
        public async Task<IActionResult> UpdateTime([FromBody]Business business)
        {
            await Service.UpdateAsync(business, new[] { "BusinessStartTime", "BusinessEndTime", "BusinessStartTime2", "BusinessEndTime2", "BusinessStartTime3", "BusinessEndTime3", });
            return Json(new JsonData { Success = true, Msg = "修改成功" });
        }

        /// <summary>
        /// 根据商户id获取商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProducts(int id, [FromServices]IProductRepository rep)
        {
            var products = await rep.GetProductTreeAsync(id, true);
            return Json(products);
        }

        /// <summary>
        /// 上下架商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        [HttpPut("products/{id}")]
        public IActionResult PutProduct(int id, [FromQuery]ProductStatus status, [FromServices]IProductRepository rep)
        {
            if (status == ProductStatus.Sale) rep.Up(id);
            else rep.Down(id);
            rep.Commit();
            return Ok("操作成功");
        }

        /// <summary>
        /// 获取商户到自提订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("selfTakeOrders/{id}")]
        public async Task<IActionResult> GetSelfTakeOrder(int id, [FromQuery]DateTime? date, [FromServices]IOrderRepository rep)
        {
            return Json(await rep.GetOrderByStatus(id, DeliveryMode.Self, startTime: date, endTime: date));
        }

        [HttpGet("sendMsg/{id}")]
        public async Task<IActionResult> GetSendMsg(int id, [FromServices]IUtilRepository res, [FromServices]IConfiguration config)
        {
            var order = await res.GetAsync<Order>(id);
            order.Status = OrderStatus.DistributorReceipt;
            await res.CommitAsync();
            await res.SendTakeOrderMsgAsync(order, config["appData:key1"], config["appData:key2"], config["appData:key3"], config["appData:key4"]);
            return Ok("通知成功");
        }

        /// <summary>
        /// 客户取走订单后的操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        [HttpPut("takenAway/{id}")]
        public async Task<IActionResult> PutTakenAway(int id, [FromServices]IOrderRepository rep)
        {
            var order = new Order { ID = id, Status = OrderStatus.Achieve };
            var res = await rep.UpdateAsync(order, new List<string> { nameof(order.Status) });
            return Json(new JsonData { Success = res > 0, Msg = "修改成功" });
        }

    }
}