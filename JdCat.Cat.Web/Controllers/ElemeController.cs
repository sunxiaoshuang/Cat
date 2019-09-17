using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Enum;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JdCat.Cat.Web.Controllers
{
    public class ElemeController : Controller
    {
        public static ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(ElemeController));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // get访问为测试接口可用性请求，直接返回ok
            if (Request.Method.ToLower() == "get")
            {
                context.Result = Json(new { message = "ok" });
                return;
            }
            base.OnActionExecuting(context);
        }

        protected IThirdOrderRepository Service { get; set; }
        public async Task<IActionResult> Notify([FromBody]JObject message, [FromServices]IThirdOrderRepository service)
        {
            Service = service;
            var appId = message["appId"].Value<long>();
            var secret = await service.GetElemeAppSecretAsync(appId);
            if(secret == null) return Json(new { message = "ok" });
            var eleme = new ElemeInputData(message, secret);
            if (!eleme.CheckSign())
            {
                // 签名验证不通过，不再继续处理
                return Json(new { message = "ok" });
            }
            var type = message["type"].Value<int>();
            switch (type)
            {
                case 10: await OrderAsync(message); break;             // 订单生效
                case 12: await ReceviedAsync(message); break;          // 商户接单
                case 14: await CancelAsync(message); break;            // 订单被取消
                case 18: await FinishAsync(message); break;            // 订单已完成
                case 20: await ApplyCancelAsync(message); break;       // 用户申请取消订单
                case 23: await CancelAsync(message); break;            // 门店同意取消订单
                case 30: await ApplyCancelAsync(message); break;       // 用户申请退单
                case 33: await CancelAsync(message); break;            // 门店同意退单
                default:
                    break;
            }

            return Json(new { message = "ok" });
        }

        /// <summary>
        /// 新订单通知
        /// </summary>
        /// <returns></returns>
        private async Task OrderAsync(JObject message)
        {
            await Service.ElemeSaveAsync(message);
        }

        /// <summary>
        /// 已接收订单
        /// </summary>
        /// <returns></returns>
        private async Task ReceviedAsync(JObject message)
        {
            var order = await Service.ElemeReceivedAsync(message);
            if (order == null) return;
            order.PrintType = PrintMode.All;
            await Service.AddOrderNotifyAsync(order);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        /// <returns></returns>
        private async Task FinishAsync(JObject message)
        {
            await Service.ElemeFinishAsync(message);
        }

        /// <summary>
        /// 订单取消
        /// </summary>
        /// <returns></returns>
        private async Task CancelAsync(JObject message)
        {
            await Service.ElemeCancelAsync(message);
        }

        /// <summary>
        /// 用户申请取消订单
        /// </summary>
        /// <returns></returns>
        private async Task ApplyCancelAsync(JObject message)
        {
            //Log.Debug(message.ToString());
            //var orderId = message["message"]["orderId"].Value<string>();
            //var order = await Service.GetOrderByCodeAsync(orderId);
            // todo 打印通知

        }

        ///// <summary>
        ///// 退单（订单完成后，用户退款）
        ///// </summary>
        ///// <returns></returns>
        //private async Task RefundAsync(JObject message)
        //{
        //    await Service.ElemeCancelAsync(message);
        //}

        ///// <summary>
        ///// 用户申请退单
        ///// </summary>
        ///// <returns></returns>
        //private async Task ApplyRefundAsync(JObject message)
        //{
        //    var orderId = message["message"]["orderId"].Value<string>();
        //    var order = await Service.GetOrderByCodeAsync(orderId);
        //    // todo 打印通知

        //}


    }
}