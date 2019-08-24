using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace JdCat.Cat.Web.Controllers
{
    public class MeituanController : Controller
    {
        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(MeituanController));
            }
        }

        public string Body { get; set; }

        ///// <summary>
        ///// 请求体
        ///// </summary>
        //protected string Body { get; set; }
        /// <summary>
        /// 美团回调参数
        /// </summary>
        protected Dictionary<string, string> formDic = new Dictionary<string, string>();

        /// <summary>
        /// 保存美团应用appid与key
        /// </summary>
        private static Dictionary<string, string> _appDic = new Dictionary<string, string>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var method = Request.Method.ToLower();
            if (method == "get" && Request.QueryString.HasValue)
            {
                foreach (var item in Request.Query)
                {
                    formDic.Add(item.Key, item.Value.ToString().ToUrlDecoding().ToUrlDecoding());
                }
            }
            if (method == "post")
            {
                var type = Request.Headers["Content-Type"];
                var exist = Request.Headers.TryGetValue("Content-Type", out StringValues vs);

                if (exist && vs[0].Contains("application/x-www-form-urlencoded"))
                {
                    foreach (var item in Request.Form)
                    {
                        formDic.Add(item.Key, item.Value.ToString().ToUrlDecoding().ToUrlDecoding());
                    }
                }
                else
                {
                    using (var stream = new StreamReader(Request.Body))
                    {
                        Body = stream.ReadToEnd();
                        if (!string.IsNullOrEmpty(Body))
                        {
                            Body = Body.ToUrlDecoding().ToUrlDecoding().TrimStart('?');
                            foreach (var item in Body.Split('&'))
                            {
                                var keyValue = item.Split('=');
                                formDic.Add(keyValue[0], keyValue[1]);
                            }
                        }
                    }
                }
            }
            // 如果没有任何请求参数，则直接返回成功
            if (formDic.Count == 0)
            {
                context.Result = Json(new { data = "ok" });
                return;
            }


            var isSuccess = ValidateSign();
            if (!isSuccess)
            {
                // 验证不成功，则直接返回
                context.Result = Json(new { data = "ok" });
                return;
            }

            //using (var st = new StreamReader(Request.Body))
            //{
            //    Body = st.ReadToEnd();
            //    if (string.IsNullOrEmpty(Body) && !Request.QueryString.HasValue)
            //    {
            //        Log.Debug($"Body={Body}，querystring={Request.QueryString}");
            //        // 没有请求体，直接返回ok
            //        context.Result = Json(new { data = "ok" });
            //        return;
            //    }
            //}
            //var isSuccess = false;
            //if (Request.Method.ToLower() == "get")
            //{
            //    isSuccess = ValidateSign(Request.QueryString.Value.Trim('?'), Request.Query["app_id"]);
            //}
            //else
            //{
            //    var appId = Regex.Match(Body, @"app_id=(\d+)").Groups[1].Value;
            //    Log.Debug(appId);
            //    if (string.IsNullOrEmpty(appId))
            //    {
            //        context.Result = Json(new { data = "ok" });
            //        return;
            //    }
            //    isSuccess = ValidateSign(Body, Request.Form["app_id"]);
            //}
            //if (!isSuccess)
            //{
            //    // 验证签名不成功，直接返回
            //    context.Result = Json(new { data = "ok" });
            //    return;
            //}
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <returns></returns>
        private bool ValidateSign()
        {
            var appId = formDic["app_id"];
            if (!_appDic.Keys.Contains(appId))
            {
                var service = HttpContext.RequestServices.GetService<IThirdOrderRepository>();
                var keyTask = service.GetMTAppKeyAsync(appId);
                keyTask.Wait();
                var key = keyTask.Result;
                if (string.IsNullOrEmpty(key)) return false;
                _appDic.Add(appId, key);
            }
            var appKey = _appDic[appId];
            var host = $"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}";
            var mt = new MTInputData(appKey, host);
            foreach (var item in formDic)
            {
                if (item.Key == "sig") continue;
                mt.SetValue(item.Key, item.Value);
            }
            var sign = mt.MakeSign();
            //Log.Debug($"{host}，sig={formDic["sig"]}，sign={sign}，{formDic["sig"] == sign}");
            return sign == formDic["sig"];
        }

        ///// <summary>
        ///// 验证签名
        ///// </summary>
        ///// <param name="querystring"></param>
        ///// <param name="appId"></param>
        ///// <returns></returns>
        //private bool ValidateSign(string querystring, string appId)
        //{
        //    if (!_appDic.Keys.Contains(appId))
        //    {
        //        var service = HttpContext.RequestServices.GetService<IThirdOrderRepository>();
        //        var key = service.GetMTAppKey(appId);
        //        if (string.IsNullOrEmpty(key)) return false;
        //        _appDic.Add(appId, key);
        //    }
        //    var appKey = _appDic[appId];
        //    var items = querystring.Split('&');
        //    var host = $"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}";
        //    var mt = new MTInputData(appKey, host);
        //    var sign = string.Empty;
        //    foreach (var item in items)
        //    {
        //        var vals = item.Split('=');
        //        if (vals[0] == "sig")
        //        {
        //            sign = vals[1];
        //            continue;
        //        }
        //        mt.SetValue(vals[0], vals[1]);
        //    }
        //    var sig = mt.MakeSign();
        //    Log.Debug($"{host}，query={querystring}，sig={sig}，sign={sign}，{sig == sign}");
        //    return sig == sign;
        //}

        public IActionResult Callback()
        {
            //Log.Debug(Body.ToUrlDecoding().ToUrlDecoding());
            return Json(new { data = "ok" });
        }

        /// <summary>
        /// 新订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Order([FromServices]IThirdOrderRepository service)
        {
            var business = await service.GetBusinessByMtPoi(formDic["app_poi_code"]);
            if (business == null || !business.MT_AutoRecieved) return Json(new { data = "ok" });
            // 如果设置了美团自动接单，则调用商户确认接口
            var url = "https://waimaiopen.meituan.com/api/v1/order/confirm";
            var mt = new MTInputData(business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", business.MT_AppId);
            mt.SetValue("order_id", formDic["order_id"]);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            await UtilHelper.RequestAsync(url, method: "get");
            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 已确定订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Received([FromServices]IThirdOrderRepository service)
        {
            //Log.Debug("美团订单：" + formDic["wm_poi_name"] + "，" + formDic["day_seq"]);
            var order = await service.MT_SaveAsync(formDic);
            if (order == null) return Json(new { data = "ok" });
            // 发送订单通知
            await service.AddOrderNotifyAsync(order);

            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 已完成
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Finish([FromServices]IThirdOrderRepository service)
        {
            var id = formDic["order_id"];
            var order = await service.MT_FinishAsync(id);
            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 催单
        /// </summary>
        /// <returns></returns>
        public IActionResult Urge()
        {
            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 隐私降级
        /// </summary>
        /// <returns></returns>
        public IActionResult Downgrade()
        {
            Log.Debug("隐私降级");
            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Cancel([FromServices]IThirdOrderRepository service)
        {
            var id = formDic["order_id"];
            var reason = formDic["reason"];
            var order = await service.MT_CancelAsync(id, reason);
            // todo 取消订单通知（美团无订单申请取消推送）


            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 部分退款
        /// </summary>
        /// <returns></returns>
        public IActionResult PartRefund()
        {
            Log.Debug("部分退款：" + Request.QueryString.Value?.ToUrlDecoding());
            return Json(new { data = "ok" });
        }
        /// <summary>
        /// 全部退款
        /// </summary>
        /// <returns></returns>
        public IActionResult AllRefund()
        {
            Log.Debug("全部退款：" + Request.QueryString.Value?.ToUrlDecoding());
            return Json(new { data = "ok" });
        }


    }
}
