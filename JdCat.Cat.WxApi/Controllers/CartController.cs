using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseController<IUserRepository, User>
    {
        public CartController(IUserRepository service) : base(service)
        {
        }

        /// <summary>
        /// 获取用户购物车记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery]int businessId)
        {
            var list = await Service.GetCartsAsync(businessId, id);
            return Json(list);
        }

        /// <summary>
        /// 新增购物车，返回新增的购物车id
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ShoppingCart cart)
        {
            await Service.AddAsync(cart);
            return Json(cart.ID);
        }

        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ShoppingCart cart)
        {
            if(cart.Quantity > 0)
            {
                Service.UpdateCart(cart);
            }
            else
            {
                await Service.DeleteAsync(cart);
            }
            return Ok("success");
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUserCart([FromQuery]int userId, [FromQuery]int businessId)
        {
            var rows = await Service.ClearCartAsync(userId, businessId);
            var result = new JsonData { Success = rows > 0 };
            if (!result.Success) result.Msg = "购物车已经清空，请勿重复清理";
            return Json(result);
        }

    }
}