﻿using System;
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
using JdCat.Cat.Model.Enum;
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
        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            Log.Debug("querystring:" + Request.QueryString.Value);
            using (var st = new StreamReader(Request.Body))
            {
                Log.Debug("body:" + st.ReadToEnd());
            }
            foreach (var item in Request.Headers)
            {
                Log.Debug(item.Key + ":" + item.Value);
            }
            return View();
        }

        /// <summary>
        /// 达达订单回调
        /// </summary>
        /// <param name="dada"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public IActionResult DadaCallback([FromBody]DadaCallBack dada)
        {
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            try
            {
                service.UpdateOrderStatus(dada);
            }
            catch (Exception e)
            {
                Log.Error("达达订单更新异常：" + e.Message);
            }
            return Ok();
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
                //log.Debug("点我达回调：" + JsonConvert.SerializeObject(dwd));
                // 每次回调时，均重新读取一次订单配送费用，并保存到数据库
                var priceResult = await helper.GetOrderPriceAsync(orderCode);
                double cost = 0;
                if (priceResult.success)
                {
                    cost = priceResult.result.receivable_price / 100;
                    log.Debug($"编号：{orderCode}，点我达配送金额为{cost}");
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
        /// 一城飞客订单状态回调
        /// </summary>
        /// <param name="ycfk"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult YcfkCallback([FromServices]IOrderRepository service, [FromServices]AppData appData, [FromServices]YcfkHelper helper)
        {
            //var orderId = Request.Form["OrderId"];
            //var orderState = Request.Form["OrderState"];
            //var reason = Request.Form["Reason"];
            //Log.Info(orderId + "|" + orderState + "|" + reason);
            //var ycfk = new YcfkCallback { OrderId = orderId, OrderState = int.Parse(orderState), Reason = reason };
            var action = Request.Form["action"];
            //var sign = Request.Form["action"];
            //var ts = Request.Form["action"];
            //var key = Request.Form["action"];
            var content = Request.Form["content"];
            if (action.ToString().ToLower() == "sendorderstate")
            {
                //if (key != appData.YcfkPartnerKey) return Json(new { StateCode = 1, StateMsg = "key值不存在" });
                //var signature = helper.CreateSignature(content, ts);
                //if (sign != signature) return Json(new { StateCode = 2, StateMsg = "签名验证失败" });

                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(content));
                var ycfk = JsonConvert.DeserializeObject<YcfkCallback>(json);
                //Log.Info(JsonConvert.SerializeObject(ycfk));
                try
                {
                    service.UpdateOrderStatus(ycfk);
                }
                catch (Exception e)
                {
                    Log.Error("一城飞客订单更新异常：" + e.Message);
                }
            }
            else if(action.ToString().ToLower() == "sendwluserlocation")
            {
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(content));
                var data = JObject.Parse(json);
                if (data == null || string.IsNullOrEmpty(data["OrderId"].ToString()))
                {
                    Log.Error("一城飞客骑手位置更新异常，原因：没有得到推送数据，得到的数据为" + json);
                }
                else
                {
                    var orderCode = data["OrderId"].ToString().Split('_')[0];
                    var id = service.GetOrderIdByCode(orderCode);
                    var ycfk = new YcfkLocation { Lat = (double)data["Lat"], Lng = (double)data["Lng"], OrderId = id };
                    service.Add(ycfk);
                }

            }
            return Json(new { StateCode = 0, StateMsg = "处理成功" });
        }
        /// <summary>
        /// 微信公众号绑定OpenId页
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        //public async Task<IActionResult> WxUser([FromQuery]string code, [FromQuery]string state)
        //{
        //    var appId = "wx37df4bb420888824";                       // 公众号AppId
        //    var secret = "8db34ed73016a5f22878295ed409cc52";        // 公众号密钥
        //    var tokenUrl = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
        //    using (var client = new HttpClient())
        //    {
        //        var res = await client.GetAsync(tokenUrl);
        //        var content = await res.Content.ReadAsStringAsync();
        //        var json = JObject.Parse(content);
        //        var openId = json["openid"].Value<string>();
        //        ViewBag.body = openId;
        //    }
        //    return View();
        //}
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
        public async Task<IActionResult> WechatCallback([FromQuery]string state)
        {
            if(!string.IsNullOrEmpty(state))
            {
                switch (state)
                {
                    case "mp":
                        // 公众号管理平台验证
                        var c = Request.Query["code"].ToString();
                        ViewBag.Url = HttpContext.RequestServices.GetService<AppData>().MpUrl;
                        return View("WeixinWebview", await WxHelper.GetOpenIdAsync(c));
                    default:
                        break;
                }
            }
            var code = Request.Query["echoStr"].ToString();

            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var result = UtilHelper.ReadXml<WxEvent>(content);
                        var service = HttpContext.RequestServices.GetService<IUtilRepository>();
                        await service.WxMsgHandlerAsync(result);
                        //if (result.MsgType == "event" && result.Event == "SCAN")
                        //{
                        //    await Listen(result);
                        //}
                        //else if(result.MsgType == "event" && result.Event == "submit_membercard_user_info")
                        //{

                        //}
                    }
                    catch (Exception e)
                    {
                        Log.Error("微信公众号回调异常：" + e);
                    }
                }
            }

            return Content(code);
        }

        /// <summary>
        /// 易联云打印回调
        /// </summary>
        /// <returns></returns>
        public IActionResult YLYPrint()
        {
            return Json(new { data = "OK" });
        }

        /// <summary>
        /// 外卖管家打印回调
        /// </summary>
        /// <returns></returns>
        public IActionResult WMGJ()
        {
            return Json(new { data = "OK" });
        }


        #region 微信事件
        //private async Task Listen(WxEvent e)
        //{
        //    //var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
        //    if (!int.TryParse(e.EventKey, out int businessId))
        //    {
        //        return;
        //    }
        //    var service = HttpContext.RequestServices.GetService<IBusinessRepository>();
        //    var users = service.GetWxListenUser(businessId);
        //    if (users.Count >= 4) return;

        //    var token = await WxHelper.GetTokenAsync(WxHelper.WeChatAppId, WxHelper.WeChatSecret);
        //    var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={token}&openid={e.FromUserName}&lang=zh_CN";
        //    using (var client = new HttpClient())
        //    {
        //        var res = client.GetAsync(url);
        //        var result = await res.Result.Content.ReadAsStringAsync();
        //        var user = JsonConvert.DeserializeObject<WxListenUser>(result);
        //        if (users.Exists(a => a.openid == user.openid))
        //        {
        //            return;
        //        }
        //        user.BusinessId = businessId;
        //        service.BindWxListen(user);
        //    }
        //}

        //private async Task MemberActiveAsync(WxEvent e)
        //{
        //    var res = await WxHelper.GetMemberInfoAsync(e.CardId, e.UserCardCode);
        //    var json = JObject.Parse(res);
        //    if(json["errcode"].ToString() != "0")
        //    {
        //        Log.Error($"获取会员[{e.UserCardCode}]信息失败：{json["errmsg"]}");
        //        return;
        //    }
        //    var service = HttpContext.RequestServices.GetService<ICardRepository>();
        //    var card = await service.GetCardAsync(e.CardId);
        //    var entity = new WxMember {
        //        Code = e.UserCardCode,
        //        NickName = json["nickname"].ToString(),
        //        CardId = e.CardId,
        //        WxCardId = card.ID,
        //        BusinessId = card.BusinessId
        //    };
        //    if(json["user_info"]["common_field_list"] != null)
        //    {
        //        var arr = json["user_info"]["common_field_list"].ToArray();
        //        entity.Name = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_NAME")?["value"].ToString();
        //        entity.Phone = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_MOBILE")?["value"].ToString();
        //        var gender = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_SEX")?["value"].ToString();
        //        if (gender == "MALE") entity.Gender = UserGender.Male;
        //        else if (gender == "FEMALE") entity.Gender = UserGender.Female;
        //        var birthday = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_BIRTHDAY")?["value"].ToString();
        //        if (birthday != null) entity.Birthday = Convert.ToDateTime(birthday);
        //    }
        //    await service.AddAsync(entity);
        //}
        #endregion

        #region 授权后跳转页面
        public IActionResult WeixinWebview()
        {
            return View();
        }
        #endregion

    }
}