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


    }
}