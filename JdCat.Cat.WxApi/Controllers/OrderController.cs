﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using JdCat.Cat.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : BaseController<IOrderRepository, Order>
    {
        private static JsonSerializerSettings JsSetting { get; set; } = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public OrderController(IOrderRepository service) : base(service)
        {

        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("getOrder/{id}")]
        public IActionResult GetOrder(int id, [FromQuery]int businessId, [FromQuery]DateTime? createTime, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            int? userId;
            if (id == 0)
            {
                userId = null;
            }
            else
            {
                userId = id;
            }
            var orders = Service.GetOrder(new Business { ID = businessId }, null, query, null, null, userId, createTime: createTime);
            result.Data = new
            {
                list = orders,                          // 订单列表
                rows = query.RecordCount                // 总记录数
            };

            result.Success = true;
            return Json(result, JsSetting);
        }

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <returns></returns>
        [HttpGet("single/{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = Service.GetOrderIncludeProduct(id);
            return Json(order, JsSetting);
        }
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <returns></returns>
        [HttpGet("singleByCode")]
        public IActionResult GetOrder([FromQuery]string code)
        {
            var order = Service.GetOrderByCode(code);
            return Json(order, JsSetting);
        }

        [HttpPost("createOrder")]
        public IActionResult CreateOrder([FromBody]Order order)
        {
            var result = Service.CreateOrder(order);
            return Json(result, JsSetting);
        }

        private static string unifieUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        [HttpGet("unifiePayment/{id}")]
        public async Task<IActionResult> UnifiePayment(int id, [FromQuery]int businessId, [FromQuery]int userId, [FromServices]AppData appData)
        {
            var business = Service.Set<Business>().First(a => a.ID == businessId);
            var user = Service.Set<User>().First(a => a.ID == userId);
            var order = Service.GetOrderIncludeProduct(id);
            var option = new WxUnifiePayment
            {
                appid = appData.ServerAppId,
                mch_id = appData.ServerMchId,
                sub_appid = business.AppId,
                sub_mch_id = business.MchId,
                sub_openid = user.OpenId,
                out_trade_no = order.OrderCode,
                total_fee = (int)Math.Round(order.Price.Value * 100, 0),
                key = appData.ServerKey,
                notify_url = appData.PaySuccessUrl,
                spbill_create_ip = appData.HostIpAddress
            };
            if (business.ID == 1)
            {
                option.total_fee = 1;
            }
            option.Generator();
            var xml = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                UtilHelper.XmlSerializeInternal(stream, option);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }
            //UtilHelper.Log(xml);

            using (var hc = new HttpClient())
            {
                var sc = new StringContent(xml);
                var response = await hc.PostAsync(unifieUrl, sc);
                var content = await response.Content.ReadAsStringAsync();
                var ret = UtilHelper.ReadXml<WxUnifieResult>(content);

                var result = new JsonData();
                if (ret.return_code == "FAIL")
                {
                    result.Msg = ret.return_msg;
                }
                else if (ret.result_code == "FAIL")
                {
                    result.Msg = ret.err_code_des;
                }
                else
                {
                    result.Success = true;
                    var payment = new WxPayment
                    {
                        appId = business.AppId,
                        package = "prepay_id=" + ret.prepay_id,
                        key = appData.ServerKey
                    };
                    // 保存支付标识码
                    order.PrepayId = ret.prepay_id;
                    Service.Commit();
                    payment.Generator();
                    result.Data = payment;
                }
                return Json(result);
            }
        }

        [HttpPost("paySuccess")]
        public IActionResult PaySuccess([FromServices]AppData appData)
        {
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                var ret = UtilHelper.ReadXml<WxPaySuccess>(content);
                if (string.IsNullOrEmpty(ret.transaction_id)) return NotFound("支付不成功");
                var order = Service.PaySuccess(ret);
                if (order != null)
                {
                    Task.Run(async () =>
                    {
                        using (var hc = new HttpClient())
                        {
                            await hc.GetAsync($"{appData.OrderUrl}/api/notify/{order.BusinessId}?code={order.OrderCode}");
                        }
                    });
                    try
                    {
                        TemplateMessage(order);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("发送模版消息出错：" + ex.Message);
                    }
                }
                else
                {
                    Log.Error(JsonConvert.SerializeObject(ret));
                    return BadRequest("简单猫订单参数错误");
                }
            }
            return Ok("ok");
        }

        [HttpGet("recevice/{id}")]
        public IActionResult AutoRecevice(int id)
        {
            var result = new JsonData();
            result.Success = Service.Receive(id);
            if (result.Success)
            {
                result.Msg = "接单成功";
            }
            else
            {
                result.Msg = "接单失败";
            }
            return Json(result);
        }

        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="order"></param>
        private void TemplateMessage(Order order)
        {
            if (string.IsNullOrEmpty(order.PrepayId)) return;
            var businessRep = HttpContext.RequestServices.GetService<IBusinessRepository>();
            var business = businessRep.Get(a => a.ID == order.BusinessId);
            if (string.IsNullOrEmpty(business.TemplateNotifyId)) return;
            var msg = new WxTemplateMessage
            {
                emphasis_keyword = "keyword2.DATA",
                template_id = business.TemplateNotifyId,
                touser = order.OpenId,
                form_id = order.PrepayId,
                page = "pages/order/orderInfo/orderInfo?id=" + order.ID
            };
            var token = WxHelper.GetTokenAsync(business.AppId, business.Secret);
            token.Wait();
            msg.access_token = token.Result;
            msg.data = new
            {
                keyword1 = new { value = order.OrderCode },
                keyword2 = new { value = order.Price + "元" },
                keyword3 = new { value = order.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") },
                keyword4 = new { value = order.ReceiverAddress },
                keyword5 = new { value = order.ReceiverName },
                keyword6 = new { value = order.Phone }
            };

            var res = WxHelper.SendTemplateMessage(msg);
            res.Wait();
            var content = res.Result;
            Log.Debug(JsonConvert.SerializeObject(content));

        }

    }
}
