using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Common.Weixin;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Repository.Service;
using JdCat.Cat.Web.App_Code;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web.Controllers
{
    public class WeixinController : Controller
    {
        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(WeixinController));
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAppMenu([FromServices]AppData appData, [FromServices]IUtilRepository util)
        {
            var business = HttpContext.Session.Get<Business>(appData.Session);
            if (business == null) throw new Exception("请先登录");
            var token = await util.GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            var menu = await WxHelper.GetAppMenuAsync(token);
            return Json(menu);
        }

        public async Task<IActionResult> CreateAppMenu([FromBody]List<WxMenu> menus, [FromServices]AppData appData, [FromServices]IUtilRepository util)
        {
            var business = HttpContext.Session.Get<Business>(appData.Session);
            if (business == null) throw new Exception("请先登录");
            var token = await util.GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            await WxHelper.DeleteAppMenuAsync(token);
            if (menus == null || menus.Count == 0) return Ok();
            var list = new List<object>();
            menus.ForEach(menu =>
            {
                if (menu.type == WxMenuCategory.none)
                {
                    var subList = new List<object>();
                    if (menu.sub_button != null && menu.sub_button.Count > 0)
                    {
                        menu.sub_button.ForEach(a => subList.Add(new { type = a.type.ToString(), a.name, a.key, url = FilterUrl(a.url), a.appid, a.pagepath, a.media_id }));
                    };
                    list.Add(new
                    {
                        menu.name,
                        sub_button = subList
                    });
                    return;
                }
                list.Add(new { type = menu.type.ToString(), menu.name, menu.key, url = FilterUrl(menu.url), menu.appid, menu.pagepath, menu.media_id });
            });
            var result = await WxHelper.CreateAppMenuAsync(list, token);
            return Content(result);
        }

        private string FilterUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var business = HttpContext.Session.Get<Business>(AppSetting.AppData.Session);
            var query = "id=" + business.ID;
            if (url.Contains(query)) return url;
            if (url.Contains("?")) return url + "&id=" + business.ID;
            return url + "?id=" + business.ID;
        }


        #region 开放平台

        public IActionResult WxMsgTest()
        {
            foreach (var item in Request.Query)
            {
                Log.Debug(item.Key + ":" + item.Value);
            }
            return Content("ok");
        }

        /// <summary>
        /// 开放平台授权事件推送URL，每十分钟推送一次
        /// </summary>
        /// <returns></returns>
        public IActionResult Event()
        {
            var wxcpt = HttpContext.RequestServices.GetService<WXBizMsgCrypt>();
            var signature = Request.Query["signature"];
            var timestamp = Request.Query["timestamp"];
            var nonce = Request.Query["nonce"];
            var encrypt_type = Request.Query["encrypt_type"];
            var msg_signature = Request.Query["msg_signature"];

            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                var sMsg = "";
                var ret = 0;
                ret = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, content, ref sMsg);
                if (ret != 0)
                {
                    Log.Error("开放平台授权推送事件错误，code:" + ret);
                }
                else
                {
                    var document = XDocument.Parse(sMsg);
                    var ticketNode = document.Root.Elements("ComponentVerifyTicket");
                    if (ticketNode != null && ticketNode.Count() > 0)
                    {
                        var ticket = ticketNode.First().Value;
                        var util = HttpContext.RequestServices.GetService<IUtilRepository>();
                        util.SetOpenTicketAsync(ticket);
                        //Log.Debug("ticket:" + ticket);
                    }
                }
            }
            return Content("success");
        }

        /// <summary>
        /// 开放平台消息管理action
        /// </summary>
        /// <returns></returns>
        public IActionResult Message(string id)
        {
            var wxcpt = HttpContext.RequestServices.GetService<WXBizMsgCrypt>();
            var signature = Request.Query["signature"];
            var timestamp = Request.Query["timestamp"];
            var nonce = Request.Query["nonce"];
            var openid = Request.Query["openid"];
            var encrypt_type = Request.Query["encrypt_type"];
            var msg_signature = Request.Query["msg_signature"];
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var msg = sr.ReadToEnd();
                var sMsg = "";
                var ret = 0;
                ret = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, msg, ref sMsg);     // 将消息解密
                if (ret != 0)
                {
                    Log.Error("客户消息处理错误");
                }
                else
                {
                    var document = XDocument.Parse(sMsg);

                    var even = UtilHelper.ReadXml<WxEvent>(sMsg);

                    // 公众号（授权后）消息处理
                    //if (!string.IsNullOrEmpty(even.Event) && !string.IsNullOrEmpty(even.MsgType))
                    //{
                    //    var service = HttpContext.RequestServices.GetService<IUtilRepository>();
                    //    await service.WxMsgHandlerAsync(even);

                    //    var code = Request.Query["echoStr"].ToString();
                    //    return Content(code);
                    //}


                    if (even.Content == "TESTCOMPONENT_MSG_TYPE_TEXT")   // 模拟接到消息后直接返回
                    {
                        document.Root.Elements("Content").First().Value = "TESTCOMPONENT_MSG_TYPE_TEXT_callback";
                        return Content(document.ToString());
                    }
                    else if (even.Content.Contains("QUERY_AUTH_CODE"))
                    {
                        // 模拟接到消息后，暂停5秒，然后调用接口发送消息
                        Thread.Sleep(4000);
                        ReturnMsg(even.Content.Split(':')[1], even.FromUserName);
                        return Content("");
                    }
                }
            }

            return Content("success");
        }

        /// <summary>
        /// 检测用回复粉丝消息
        /// </summary>
        /// <param name="code">授权码</param>
        /// <param name="openId">用户openid</param>
        private async void ReturnMsg(string code, string openId)
        {
            try
            {
                var util = HttpContext.RequestServices.GetService<IUtilRepository>();

                var token = await WxHelper.GetAuthTokenAsync(code, await util.GetOpenTokenAsync());
                var url = $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={token.authorization_info.authorizer_access_token}";
                var content = new
                {
                    touser = openId,
                    msgtype = "text",
                    text = new { content = code + "_from_api" }
                };
                var result = await UtilHelper.RequestAsync(url, content);
                var sendData = JsonConvert.SerializeObject(content);
                Log.Debug(result);
            }
            catch (Exception e)
            {
                Log.Debug("回复消息错误：", e);
            }
        }

        /// <summary>
        /// 设置ticket，测试用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IActionResult SetCode([FromQuery]string code, [FromServices]IUtilRepository util)
        {
            util.SetOpenTicketAsync(code);
            return Content("ok");
        }

        #endregion

    }
}