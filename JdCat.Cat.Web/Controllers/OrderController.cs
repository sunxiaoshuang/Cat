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
        public async Task<IActionResult> Send(int id, [FromQuery]int type, [FromServices]DadaHelper helper)
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
                    order.ErrorReason = back.msg;
                    Service.Commit();
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

        /// <summary>
        /// 打印小票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="device_no"></param>
        /// <returns></returns>
        public async Task<IActionResult> Print(int id, [FromQuery]string device_no)
        {
            var order = Service.GetOrderIncludeProduct(id);
            var result = await Print(order, device_no);
            return Json(result);
        }

        /// <summary>
        /// ws消息处理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MessageHandler([FromQuery]string code, [FromServices]DadaHelper helper)
        {
            // Data记录语音提示方式，Msg记录提示语
            var result = new JsonData();
            if (!Business.IsAutoReceipt)
            {
                result.Success = true;
                result.Data = 1;
                result.Msg = "商家不自动接单";
                return Json(result);
            }

            var order = Service.GetOrderByCode(code);
            switch (Business.ServiceProvider)
            {
                case ServiceProvider.None:
                    order.Status = OrderStatus.Receipted;
                    result.Data = 2;
                    result.Success = Service.Commit() > 0;
                    break;
                case ServiceProvider.Self:
                    // 自己配送
                    result.Success = Service.SendOrderSelf(order.ID, order);
                    result.Data = 2;
                    result.Msg = "自动接单成功，配送方式：自己配送";
                    break;
                case ServiceProvider.Dada:
                    // 达达配送
                    var back = await helper.SendOrderAsync(order, Business);
                    // 创建达达订单未成功
                    if (!back.IsSuccess())
                    {
                        result.Data = 3;
                        result.Msg = back.msg;
                        order.Status = OrderStatus.Receipted;
                        order.ErrorReason = back.msg;
                        Service.Commit();
                    }
                    else
                    {
                        result.Success = Service.SendSuccess(order, back);
                        result.Data = 2;
                        result.Msg = "自动接单成功，配送方式：达达配送";
                    }
                    break;
                default:
                    break;
            }
            // 打印小票
            var json = await Print(order, Business.DefaultPrinterDevice);
            if (!json.Success)
            {
                result.Success = false;
                result.Msg += "|" + json.Msg;
            }

            return Json(result);
        }


        private async Task<JsonData> Print(Order order, string device_no)
        {
            var result = new JsonData();
            var helper = GetPrintHelper();
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
                return result;
            }
            result.Msg = "正在打印小票，请稍等";
            return result;
        }


    }
}