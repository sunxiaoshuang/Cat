using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class OrderController : BaseController<IOrderRepository, Order>
    {
        public OrderController(AppData appData, IOrderRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取订单记录
        /// </summary>
        /// <param name="status"></param>
        /// <param name="query"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetOrder([FromQuery]int status, [FromBody]PagingQuery query, [FromServices]JsonSerializerSettings setting)
        {
            var result = new JsonData();
            var state = status == 0 ? null : (OrderStatus?)status;
            var list = Service.GetOrder(Business, state, query);
            result.Data = new {
                list, rows = query.RecordCount
            };
            result.Success = true;
            return Json(result, setting);
        }

        /// <summary>
        /// 接单
        /// </summary>
        public IActionResult Receive(int id, [FromServices]JsonSerializerSettings setting)
        {
            var result = new JsonData
            {
                Success = Service.Receive(id)
            };
            result.Msg = result.Success ? "接单成功" : "接单异常或者已经接单";
            return Json(result, setting);
        }

        /// <summary>
        /// 接单
        /// </summary>
        public IActionResult Reject(int id, string msg, [FromServices]JsonSerializerSettings setting)
        {
            var result = new JsonData
            {
                Success = Service.Reject(id, msg)
            };
            result.Msg = result.Success ? "拒绝订单成功" : "拒绝订单异常或者订单已经拒绝";
            return Json(result, setting);
        }

    }
}