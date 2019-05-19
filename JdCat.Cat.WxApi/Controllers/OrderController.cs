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
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using log4net;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.WxApi.Models;

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
            var setMeals = order.Products.Where(a => a.Feature == ProductFeature.SetMeal);
            if (setMeals.Count() > 0)
            {
                Service.QuarySetMealProduct(setMeals);
            }
            return Json(order);
        }
        /// <summary>
        /// 客户端获取订单列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("getOrderFromClient/{id}")]
        public IActionResult GetOrderFromClient(int id, [FromQuery]int businessId, [FromQuery]DateTime? createTime, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            var orders = Service.GetOrder(new Business { ID = businessId }, null, query, null, null, null, createTime: createTime);
            var setMeals = orders.SelectMany(a => a.Products).Where(a => a.Feature == ProductFeature.SetMeal);
            Service.QuarySetMealProduct(setMeals);
            result.Data = new
            {
                list = orders,                          // 订单列表
                rows = query.RecordCount                // 总记录数
            };

            result.Success = true;
            return Json(result, JsSetting);
        }

        ///// <summary>
        ///// 获取订单
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("estimateFreight/{id}")]
        //public async Task<IActionResult> EstimateFreight(int id, [FromQuery]double lat, [FromQuery]double lng, [FromQuery]string address)
        //{
        //    var result = await Service.EstimateFreight(id, lat, lng, address);
        //    return Json(result);
        //}

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
            if (appData.RunMode == "test" || business.ID == 1)
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
        public async Task<IActionResult> PaySuccess([FromServices]AppData appData, [FromServices]IBusinessRepository businessRepository)
        {
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                var ret = UtilHelper.ReadXml<WxPaySuccess>(content);
                if (string.IsNullOrEmpty(ret.transaction_id)) return BadRequest("支付不成功");
                var order = Service.PaySuccess(ret);
                if (order != null)
                {
                    try
                    {
                        await Service.Print(order, business: order.Business);                 // 打印小票
                    }
                    catch (Exception e)
                    {
                        Log.Error("自动打印失败", e);
                    }
                    await Service.AutoReceipt(order);                 // 自动接单
                    await Task.Run(async () =>
                     {
                         using (var hc = new HttpClient())
                         {
                             await hc.GetAsync($"{appData.OrderUrl}/api/notify/{order.BusinessId}?code={order.OrderCode}&state={(int)order.Status}");
                         }
                     });

                    try
                    {
                        TemplateMessage(order);             // 小程序模版消息
                        await EventMessage(order, appData);       // 公众号模版消息
                    }
                    catch (Exception ex)
                    {
                        Log.Error("发送模版消息出错：" + ex.Message);
                    }
                    // 订单提醒：将数据存储在通知服务中，等待客户端来取
                    try
                    {
                        using (var hc = new HttpClient())
                        {
                            var setMeals = order.Products.Where(a => a.Feature == ProductFeature.SetMeal);
                            if (setMeals.Count() > 0)
                            {
                                Service.QuarySetMealProduct(setMeals);
                            }
                            order.Business = null;
                            order.User = null;
                            order.DadaCallBacks = null;
                            order.DadaReturn = null;
                            var body = new PostNewOrderData { BusinessId = order.BusinessId.Value, Content = JsonConvert.SerializeObject(order, AppData.JsonSetting), OrderId = order.ID };
                            var postData = new StringContent(JsonConvert.SerializeObject(body));
                            postData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            var res = await hc.PostAsync($"{appData.OrderUrl}/api/notify", postData);
                            res.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("新订单消息通知错误：" + e.Message);
                    }
                }
                else
                {
                    return BadRequest("简单猫订单参数错误");
                }
            }
            //var result = string.Empty;
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    UtilHelper.XmlSerializeInternal(stream, new { return_code = "SUCCESS", return_msg = "OK" });
            //    stream.Position = 0;
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        result = reader.ReadToEnd();
            //    }
            //}
            //return Ok(result);
            return Content("SUCCESS");
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
        /// 申请退款
        /// </summary>
        /// <returns></returns>
        [HttpGet("applyRefund/{id}")]
        public async Task<IActionResult> ApplyRefund(int id, [FromQuery]string reason)
        {
            var result = Service.ApplyRefund(id, reason);
            if (!result.Success) return Json(result);
            await Service.SendMsgOfRefund((Order)result.Data);
            return Json(result);
        }

        /// <summary>
        /// 发送模版消息，通知用户付款成功
        /// </summary>
        /// <param name="order"></param>
        private void TemplateMessage(Order order)
        {
            if (string.IsNullOrEmpty(order.PrepayId)) return;
            var openId = order.OpenId;
            if (string.IsNullOrEmpty(openId))
            {
                var rep = HttpContext.RequestServices.GetService<IUserRepository>();
                openId = rep.Get(order.UserId.Value).OpenId;
            }
            var businessRep = HttpContext.RequestServices.GetService<IBusinessRepository>();
            var business = businessRep.Get(a => a.ID == order.BusinessId);
            if (string.IsNullOrEmpty(business.TemplateNotifyId)) return;
            var msg = new WxTemplateMessage
            {
                emphasis_keyword = "keyword2.DATA",
                template_id = business.TemplateNotifyId,
                touser = openId,
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
                keyword5 = new { value = order.GetUserCall() },
                keyword6 = new { value = order.Phone }
            };

            var res = WxHelper.SendTemplateMessageAsync(msg);
            res.Wait();
            var content = res.Result;

        }
        /// <summary>
        /// 发送事件消息，通知商户订单信息
        /// </summary>
        /// <param name="order"></param>
        private async Task EventMessage(Order order, AppData appData)
        {
            if (string.IsNullOrEmpty(appData.EventMessageTemplateId)) return;
            var rep = HttpContext.RequestServices.GetService<IBusinessRepository>();
            var msg = new WxEventMessage();
            msg.template_id = appData.EventMessageTemplateId;
            var productName = string.Empty;
            foreach (var item in order.Products)
            {
                productName += item.Name + " *" + (double)item.Quantity + "、";
            }
            productName = productName.Remove(productName.Length - 1);
            var keyword5 = (order.Status == OrderStatus.Distribution || order.Status == OrderStatus.DistributorReceipt) ? "待配送" : "已付款";
            msg.data = new
            {
                first = new { value = $"流水号：   #{order.Identifier}      ￥{order.Price}" },
                keyword1 = new { value = productName },
                keyword2 = new { value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                keyword3 = new { value = order.ReceiverAddress },
                keyword4 = new { value = "   " + order.GetUserCall() + "    " + order.Phone },
                keyword5 = new { value = keyword5, color = "#ff0000" },
                remark = new { value = order.Remark }
            };
            var users = rep.GetWxListenUser(order.BusinessId.Value);
            if (users == null || users.Count == 0) return;
            foreach (var item in users)
            {
                msg.touser = item.openid;
                var result = await WxHelper.SendEventMessageAsync(msg);
                var ret = JsonConvert.DeserializeObject<WxMessageReturn>(result);
                if (ret.errcode == 40001)
                {
                    // 如果发送消息提示token失效，则重新获取token，再发送一次
                    await WxHelper.SetTokenAsync(WxHelper.WeChatAppId, WxHelper.WeChatSecret);
                    result = await WxHelper.SendEventMessageAsync(msg);
                    Log.Info(result);
                }
            }
        }

        /// <summary>
        /// 获取订单骑手位置
        /// </summary>
        /// <returns></returns>
        [HttpGet("orderLocation/{id}")]
        public IActionResult GetOrderLocation(int id)
        {
            var location = Service.GetOrderLocation(id);
            return Json(location);
        }

        /// <summary>
        /// 订单评论
        /// </summary>
        /// <returns></returns>
        [HttpPost("comment")]
        public IActionResult Comment([FromBody]OrderComment comment)
        {
            var result = Service.Comment(comment);
            Service.ReloadCommentScore(comment.BusinessId);
            return Json(result);
        }


    }
}
