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

        public async Task<IActionResult> GetAppMenu()
        {
            var menu = await WxHelper.GetAppMenuAsync();
            return Json(menu);
        }

        public async Task<IActionResult> CreateAppMenu()
        {
            var result = await WxHelper.CreateAppMenuAsync();
            return Content(result);
        }

        #region 开放平台

        public IActionResult WxMsgTest()
        {
            var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(WeixinController));

            foreach (var item in Request.Query)
            {
                log.Debug(item.Key + ":" + item.Value);
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
                        WxHelper.OpenTicket = ticket;
                        Log.Debug("ticket:" + ticket);
                    }
                }
            }
            return Content("success");
        }

        /// <summary>
        /// 消息管理action
        /// </summary>
        /// <returns></returns>
        public IActionResult Message()
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
                ret = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, msg, ref sMsg);
                if (ret != 0)
                {
                    Log.Error("客户消息处理错误");
                }
                else
                {
                    Log.Debug("客户消息：" + sMsg);
                    var document = XDocument.Parse(sMsg);
                    var content = document.Root.Elements("Content").First().Value;
                    var openId = document.Root.Elements("FromUserName").First().Value;
                    if (content == "TESTCOMPONENT_MSG_TYPE_TEXT")                // 模拟接到消息后直接返回
                    {
                        document.Root.Elements("Content").First().Value = "TESTCOMPONENT_MSG_TYPE_TEXT_callback";
                        Log.Debug(document.ToString());
                        return Content(document.ToString());
                    }
                    else
                    {
                        // 模拟接到消息后，暂停5秒，然后调用接口发送消息
                        Thread.Sleep(4000);
                        ReturnMsg(content, openId, HttpContext.RequestServices.GetService<AppData>());
                        return Content("");
                    }
                }
            }


            //log.Debug(id);
            return Content("success");
        }

        private async void ReturnMsg(string code, string openId, AppData appData)
        {
            var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(WeixinController));
            try
            {

                log.Debug("客服正在回复消息，请稍等...");
                var token = await WxHelper.GetAuthToken(appData, code);
                if (token == null)
                {
                    Log.Debug("token是空的");
                    Log.Debug("appData:" + JsonConvert.SerializeObject(appData));
                }
                log.Debug(JsonConvert.SerializeObject(token));
                var url = $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={token.authorization_info.authorizer_access_token}";
                var content = new
                {
                    touser = openId,
                    msgtype = "text",
                    text = new { content = code + "_from_api" }
                };
                var sendData = JsonConvert.SerializeObject(content);
                log.Debug("回复的消息：" + sendData);
                using (var client = new HttpClient())
                using (var body = new StringContent(sendData))
                {
                    var res = await client.PostAsync(url, body);
                    var result = await res.Content.ReadAsStringAsync();
                    Log.Debug(result);
                }
            }
            catch (Exception e)
            {
                log.Debug("回复消息错误：", e);
            }
        }

        /// <summary>
        /// 设置ticket，测试用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IActionResult SetCode([FromQuery]string code)
        {
            if (string.IsNullOrEmpty(WxHelper.OpenTicket))
            {
                WxHelper.OpenTicket = code;
            }
            return Content("ok");
        }

        #endregion
    }
}