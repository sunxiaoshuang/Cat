using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class OrderController : BaseController<IOrderRepository, Order>
    {
        public OrderController(AppData appData, IOrderRepository service) : base(appData, service)
        {

        }

        /// <summary>
        /// 订单列表页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index([FromQuery]string code, [FromServices]List<DadaCancelReason> reasonList)
        {
            ViewBag.reasonList = JsonConvert.SerializeObject(reasonList, AppData.JsonSetting);
            ViewBag.deviceList = JsonConvert.SerializeObject(Service.GetPrinters(Business), AppData.JsonSetting);
            ViewBag.code = code + "";
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
        public IActionResult GetOrder([FromQuery]int status, [FromQuery]string code, [FromQuery]string phone, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            var state = status == 0 ? null : (OrderStatus?)status;
            var list = Service.GetOrder(Business, state, query, code, phone);
            result.Data = new
            {
                list,
                rows = query.RecordCount
            };
            result.Success = true;
            return Json(result);
        }

        /// <summary>
        /// 接单
        /// </summary>
        public IActionResult Receive(int id)
        {
            var result = new JsonData
            {
                Success = Service.Receive(id)
            };
            result.Msg = result.Success ? "接单成功" : "接单异常或者已经接单";
            return Json(result);
        }

        /// <summary>
        /// 拒单
        /// </summary>
        public IActionResult Reject(int id, [FromQuery]string msg)
        {
            var result = new JsonData
            {
                Success = Service.Reject(id, msg)
            };
            result.Msg = result.Success ? "拒绝订单成功" : "拒绝订单异常或者订单已经拒绝";
            return Json(result);
        }

        /// <summary>
        /// 订单配送
        /// </summary>
        /// <param name="id"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public async Task<IActionResult> Send(int id, [FromQuery]int type, [FromServices]IHostingEnvironment environment, [FromServices]DadaHelper helper)
        {
            var result = new JsonData();

            if (type == 0)
            {
                // 达达配送
                var order = Service.Get(id);
                var back = await helper.SendOrderAsync(order, Business);
                // 创建达达订单未成功
                if (!back.IsSuccess())
                {
                    result.Msg = back.msg;
                    return Json(result);
                }
                result.Success = Service.SendSuccess(order, back);
                result.Msg = result.Success ? "配送成功" : "接单异常或者已经接单";
                result.Data = new
                {
                    Mode = DeliveryMode.Third,
                    order.Status
                };
            }
            else
            {
                // 自己配送
                result.Success = Service.SendOrderSelf(id);
                result.Msg = result.Success ? "操作成功" : "接单异常或者已经接单";
                result.Data = new
                {
                    Mode = DeliveryMode.Own,
                    Status = OrderStatus.Distribution,
                };
            }
            return Json(result);
        }

        /// <summary>
        /// 配送完成，自己配送的订单，需要在管理页面点击完成按钮
        /// </summary>
        /// <returns></returns>
        public IActionResult Achieve(int id)
        {
            var result = new JsonData
            {
                Success = Service.Achieve(id)
            };
            result.Msg = result.Success ? "配送完成" : "订单不存在";
            result.Data = OrderStatus.Achieve;
            return Json(result);
        }

        /// <summary>
        /// 取消达达配送
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flagId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<IActionResult> Cancel(int id, [FromQuery]int flagId, [FromQuery]string reason, [FromServices]DadaHelper helper)
        {
            var result = new JsonData();
            var order = Service.Get(a => a.ID == id);
            if (order.DeliveryMode == DeliveryMode.Third)
            {
                var back = await helper.CancelOrderAsync(order.OrderCode, Business, flagId, reason);
                // 取消不成功
                if (!back.IsSuccess())
                {
                    result.Msg = back.msg;
                    return Json(result);
                }
                result.Success = Service.CancelSuccess(order, back);
                result.Data = OrderStatus.CallOff;
            }
            else
            {
                order.Status = OrderStatus.Receipted;
                result.Success = Service.Commit() > 0;
                result.Data = OrderStatus.Receipted;
            }
            result.Msg = "配送取消成功";
            return Json(result);
        }

        public async Task<IActionResult> Print(int id, [FromQuery]string device_no)
        {
            var result = new JsonData();
            var helper = GetPrintHelper();
            var order = Service.GetOrderIncludeProduct(id);
            var ret = await helper.Print(device_no, order);
            result.Success = ret.ErrCode == null || ret.ErrCode == 0;
            result.Msg = ret.ErrMsg;
            if (Business.FeyinToken != helper.Token)
            {
                // 如果商户Session中保存的令牌与执行打印后的Token不一致，则修改商户中的Token
                Business.FeyinToken = helper.Token;
                HttpContext.Session.Set(AppData.Session, Business);
            }

            if (!result.Success)
            {
                // 打印失败
                return Json(result);
            }
            result.Msg = "正在打印小票，请稍等";
            return Json(result);
        }

    }
}