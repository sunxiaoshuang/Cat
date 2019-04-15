using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using JdCat.Cat.Model;
using Microsoft.Extensions.DependencyInjection;
using JdCat.Cat.Repository.Service;

namespace JdCat.Cat.Web.Controllers
{
    public class StoreController : BaseController<IStoreRepository, TangOrder>
    {
        public StoreController(AppData appData, IStoreRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 堂食订单页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Order()
        {
            return View();
        }
        
        /// <summary>
        /// 获取堂食订单列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery]PagingQuery paging, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate)
        {
            var list = await Service.GetOrdersAsync(Business.ID, paging, startDate ?? DateTime.Now, endDate ?? DateTime.Now);
            return Json(new {
                rows = list,
                count = paging.RecordCount
            });
        }
        /// <summary>
        /// 获取堂食订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await Service.GetOrderAsync(id);
            return Json(order);
        }
        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderDetail(int id)
        {
            return PartialView(await Service.GetOrderAsync(id));
        }

    }
}
