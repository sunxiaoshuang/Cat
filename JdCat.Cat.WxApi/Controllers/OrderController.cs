using System;
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
        public IActionResult GetOrder(int id, [FromQuery]int businessId, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            var orders = Service.GetOrder(new Business { ID = businessId }, null, query, null, null, id);
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

        [HttpPost("createOrder")]
        public IActionResult CreateOrder([FromBody]Order order)
        {
            var result = new JsonData();
            order = Service.CreateOrder(order);
            result.Success = true;
            result.Msg = "创建订单成功";
            result.Data = order;
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
                //appid = business.AppId,
                //mch_id = business.MchId,
                appid = appData.ServerAppId,
                mch_id = appData.ServerMchId,
                sub_appid = business.AppId,
                sub_mch_id = business.MchId,
                sub_openid = user.OpenId,
                out_trade_no = order.OrderCode,
                total_fee = (int)Math.Round(order.Price.Value * 100, 0),
                key = appData.ServerKey
            };
            if(business.ID == 1)
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
            UtilHelper.Log(xml);

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
                    payment.Generator();
                    result.Data = payment;
                }
                return Json(result);
            }
        }

        //[HttpGet("paySuccess/{id}")]
        //public IActionResult PaySuccess(int id, [FromServices]AppData appData)
        //{
        //    Order order = Service.PaySuccess(id);
        //    var result = new JsonData
        //    {
        //        Success = true,
        //        Data = order
        //    };
        //    Task.Run(async () => {
        //        using (var hc = new HttpClient())
        //        {
        //            await hc.GetAsync($"{appData.OrderUrl}/api/notify/{order.BusinessId}?code={order.OrderCode}");
        //        }
        //    });
        //    return Json(result, new JsonSerializerSettings
        //    {
        //        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
        //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    });
        //}
        [HttpPost("paySuccess")]
        public IActionResult PaySuccess([FromServices]AppData appData)
        {
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                var ret = UtilHelper.ReadXml<WxPaySuccess>(content);
                if (string.IsNullOrEmpty(ret.transaction_id)) return NotFound("支付不成功");
                var order = Service.PaySuccess(ret);
                if(order != null)
                {
                    Task.Run(async () =>
                    {
                        using (var hc = new HttpClient())
                        {
                            await hc.GetAsync($"{appData.OrderUrl}/api/notify/{order.BusinessId}?code={order.OrderCode}&status={(int)order.Status}");
                        }
                    });
                }
            }
            return Ok("ok");
        }
    }
}
