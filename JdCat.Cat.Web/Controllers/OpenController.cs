using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository.Service;
using JdCat.Cat.Web.App_Code;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web.Controllers
{
    public class OpenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 达达订单回调
        /// </summary>
        /// <param name="dada"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public IActionResult DadaCallback([FromBody]DadaCallBack dada, [FromServices]IHostingEnvironment environment)
        {
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            try
            {
                service.UpdateOrderStatus(dada);
            }
            catch (Exception e)
            {
                var filename = Path.Combine(environment.ContentRootPath, "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                System.IO.File.AppendAllText(filename, "\r\n" + Environment.NewLine + e.Message);
            }
            return Ok("更新完成");
        }
        /// <summary>
        /// 点我达订单回调
        /// </summary>
        /// <param name="dada"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public async Task<IActionResult> DWDCallback([FromBody]DWD_Callback dwd, [FromServices]DwdHelper helper)
        {
            var orderCode = dwd.order_original_id.Split('_')[0];
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            try
            {
                var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
                log.Debug("点我达回调：" + JsonConvert.SerializeObject(dwd));
                // 每次回调时，均重新读取一次订单配送费用，并保存到数据库
                var priceResult = await helper.GetOrderPrice(orderCode);
                double cost = 0;
                if (priceResult.success)
                {
                    cost = priceResult.result.receivable_price / 100;
                }
                service.UpdateOrderByDwd(dwd, cost);
            }
            catch (Exception e)
            {
                var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
                log.Info("点我达回调错误：" + e.Message);
            }


            return Json(new { success = true });
        }
        /// <summary>
        /// 微信公众号绑定OpenId页
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<IActionResult> WxUser([FromQuery]string code, [FromQuery]string state)
        {
            var appId = "wx37df4bb420888824";                       // 公众号AppId
            var secret = "8db34ed73016a5f22878295ed409cc52";        // 公众号密钥
            var tokenUrl = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(tokenUrl);
                var content = await res.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var openId = json["openid"].Value<string>();
                ViewBag.body = openId;
            }
            return View();
        }
        /// <summary>
        /// 根据商户编码绑定OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        //public IActionResult BindBusinessCode([FromQuery]string code, [FromQuery]string openId)
        //{

        //    var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
        //    log.Debug("上传的code：" + code);
        //    log.Debug("上传的openId：" + openId);
        //    var service = HttpContext.RequestServices.GetService<IBusinessRepository>();
        //    var business = service.GetBusinessByStoreId(code);
        //    var result = new JsonData();
        //    if (business == null)
        //    {
        //        result.Msg = "绑定失败，商户编码不存在";
        //        return Json(result);
        //    }
        //    business.OpendId = openId;
        //    service.Commit();
        //    result.Success = true;
        //    return Json(result);
        //}
        /// <summary>
        /// 微信事件通知
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> WechatCallback()
        {
            var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));

            var code = Request.Query["echoStr"].ToString();

            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                log.Info(content);
                try
                {
                    var result = UtilHelper.ReadXml<WxEvent>(content);
                    if (result.MsgType == "event" && result.Event == "SCAN")
                    {
                        await Listen(result);
                    }
                }
                catch (Exception e)
                {
                    log.Error("微信公众号开发平台错误：" + e);
                }
            }

            return Content(code);
        }


        #region 微信事件
        private async Task Listen(WxEvent e)
        {
            //var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
            if (!int.TryParse(e.EventKey, out int businessId))
            {
                return;
            }
            var service = HttpContext.RequestServices.GetService<IBusinessRepository>();
            var users = service.GetWxListenUser(businessId);
            if (users.Count >= 3) return;

            var token = await WxHelper.GetTokenAsync(WxHelper.WeChatAppId, WxHelper.WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={token}&openid={e.FromUserName}&lang=zh_CN";
            using (var client = new HttpClient())
            {
                var res = client.GetAsync(url);
                var result = await res.Result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<WxListenUser>(result);
                if(users.Exists(a => a.openid == user.openid))
                {
                    return;
                }
                user.BusinessId = businessId;
                service.BindWxListen(user);
            }
        }
        #endregion

    }
}