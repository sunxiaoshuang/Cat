using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JdCat.Cat.Web.Controllers
{
    public class ThirdOrderController : BaseController<IThirdOrderRepository, ThirdOrder>
    {
        public ThirdOrderController(AppData appData, IThirdOrderRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 获取商户的本地商品与第三方商品的映射
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpGet("/thirdOrder/mappings")]
        public async Task<IActionResult> GetProductMappings([FromQuery]int source)
        {
            return Json(await Service.GetProductMappingsAsync(Business.ID, source));
        }
        /// <summary>
        /// 获取商户本地商品
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpGet("/thirdOrder/products")]
        public async Task<IActionResult> GetProducts([FromServices]IProductRepository pro)
        {
            return Json(await pro.GetProductsOnlyNameAsync(Business.ID));
        }
        /// <summary>
        /// 清空第三方商品与本地商品的映射关系
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<IActionResult> Clear([FromQuery]int source)
        {
            await Service.ClearMappingsAsync(Business.ID, source);
            return Content("ok");
        }
        /// <summary>
        /// 保存映射关系
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        [HttpPost("/thirdOrder/saveMappings")]
        public async Task<IActionResult> SaveMapping([FromBody]List<ThirdProductMapping> mappings)
        {
            mappings.ForEach(a => a.BusinessId = Business.ID);
            await Service.SetProductMappingsAsync(mappings);
            return Json(await Service.GetProductMappingsAsync(Business.ID, mappings.FirstOrDefault().ThirdSource));
        }

        [HttpGet]
        public IActionResult Meituan()
        {
            return View(Business);
        }

        [HttpPost("/thirdOrder/mt/save")]
        public async Task<IActionResult> SaveMTSetting([FromBody]JObject obj)
        {
            Business.MT_AppId = obj["appId"].Value<string>();
            Business.MT_AppKey = obj["key"].Value<string>();
            Business.MT_Poi_Id = obj["poi_id"].Value<string>();
            await Service.UpdateAsync(Business, new List<string> { nameof(Business.MT_AppKey), nameof(Business.MT_AppId), nameof(Business.MT_Poi_Id) });
            SaveSession();
            return Json("ok");
        }

        [HttpGet("/thirdOrder/mt/products")]
        public IActionResult MeituanProduct()
        {
            return View();
        }

        [HttpGet("/thirdOrder/mt/getProducts")]
        public async Task<IActionResult> GetMeituanProducts([FromQuery]int offset, [FromQuery]int limit)
        {
            var url = "https://waimaiopen.meituan.com/api/v1/food/list";
            var timestamp = DateTime.Now.ToTimestamp();
            var app_id = Business.MT_AppId;
            var key = Business.MT_AppKey;
            var app_poi_code = Business.MT_Poi_Id;
            limit = limit == 0 ? 50 : limit;
            var mt = new MTInputData(key, url);
            mt.SetValue(nameof(timestamp), timestamp);
            mt.SetValue(nameof(app_id), app_id);
            mt.SetValue(nameof(app_poi_code), app_poi_code);
            mt.SetValue(nameof(limit), limit);
            mt.SetValue(nameof(offset), offset);
            var sig = mt.MakeSign();
            var api = $"{url}?{mt.ToUrl()}&sig={sig}";
            var res = await UtilHelper.RequestAsync(api, method: "get");
            var json = JObject.Parse(res);
            var items = json["data"].Select(item => new { name = item["name"].Value<string>(), pic = item["picture"].Value<string>() });

            return Json(json);
        }


        public IActionResult Orders()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Eleme()
        {
            return View(Business);
        }
        [HttpPost("/thirdOrder/eleme/save")]
        public async Task<IActionResult> SaveElemeSetting([FromBody]JObject obj)
        {
            Business.Eleme_AppId = obj["appId"].Value<long>();
            Business.Eleme_AppKey = obj["key"].Value<string>();
            Business.Eleme_AppSecret = obj["secret"].Value<string>();
            Business.Eleme_Poi_Id = obj["poi_id"].Value<long>();
            await Service.UpdateAsync(Business, new List<string> { nameof(Business.Eleme_AppId), nameof(Business.Eleme_AppKey), nameof(Business.Eleme_AppSecret), nameof(Business.Eleme_Poi_Id) });
            SaveSession();
            return Json("ok");
        }
        [HttpGet("/thirdOrder/eleme/products")]
        public IActionResult ElemeProduct()
        {
            return View();
        }
        [HttpGet("/thirdOrder/eleme/token")]
        public async Task<IActionResult> GetElemeToken()
        {
            var token = await Service.GetElemeTokenAsync(AppData.ElemeToken, Business.Eleme_AppKey, Business.Eleme_AppSecret);
            return Content(token);
        }
        [HttpGet("/thirdOrder/eleme/getProducts")]
        public async Task<IActionResult> GetElemeProducts([FromQuery]int offset, [FromQuery]int limit)
        {
            limit = limit == 0 ? 50 : limit;
            var url = AppData.ElemeApi;
            var tokenUrl = AppData.ElemeToken;
            var token = await Service.GetElemeTokenAsync(tokenUrl, Business.Eleme_AppKey, Business.Eleme_AppSecret);
            var json = new JObject
            {
                ["nop"] = "1.0.0",
                ["id"] = Guid.NewGuid().ToString(),
                ["metas"] = new JObject { ["app_key"] = Business.Eleme_AppKey, ["timestamp"] = DateTime.Now.ToTimestamp() },
                ["action"] = "eleme.product.item.queryItemByPage",
                ["token"] = token,
                //["params"] = new JObject { ["shopId"] = Business.Eleme_Poi_Id, ["offset"] = offset, ["limit"] = limit }
                ["params"] = new JObject { ["queryPage"] = new JObject { ["shopId"] = Business.Eleme_Poi_Id, ["offset"] = offset, ["limit"] = limit } }
            };
            var eleme = new ElemeInputData(json, Business.Eleme_AppSecret);
            json["signature"] = eleme.MakeSignApi();
            using (var client = new HttpClient())
            {
                var body = new StringContent(json.ToJson());
                body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.PostAsync(url, body);
                return Content(await res.Content.ReadAsStringAsync());
            }

        }

        /// <summary>
        /// 第三方订单页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Order()
        {
            return View();
        }
        /// <summary>
        /// 获取第三方订单
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetOrders([FromQuery]int source, [FromQuery]DateTime? start, [FromQuery]DateTime? end, [FromQuery]PagingQuery paging)
        {
            var list = await Service.GetOrdersAsync(source, start ?? DateTime.Now.Date, end?.AddDays(1) ?? DateTime.Now.Date.AddDays(1), paging);
            return Json(new
            {
                list,
                rows = paging.RecordCount,
                pages = paging.PageCount
            });
        }
        /// <summary>
        /// 获取订单详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(int id)
        {
            var order = await Service.GetOrderDetailAsync(id);
            return PartialView(order);
        }


        #region 美团必接的接口

        /// <summary>
        /// 商家确认订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmOrder([FromQuery]string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return Content("请添加参数：orderId");
            var url = "https://waimaiopen.meituan.com/api/v1/order/confirm";
            var mt = new MTInputData(Business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", Business.MT_AppId);
            mt.SetValue("order_id", orderId);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            var content = await UtilHelper.RequestAsync(url, method: "get");
            return Content(content);
        }

        /// <summary>
        /// 商家取消订单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CancelOrder([FromQuery]string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return Content("请添加参数：orderId");
            var url = "https://waimaiopen.meituan.com/api/v1/order/cancel";
            var mt = new MTInputData(Business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", Business.MT_AppId);
            mt.SetValue("order_id", orderId);
            mt.SetValue("reason", "用户地址填写错误");
            mt.SetValue("reason_code", 1001);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            var content = await UtilHelper.RequestAsync(url, method: "get");
            return Content(content);
        }

        /// <summary>
        /// 确认订单退款申请
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RefundAgree([FromQuery]string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return Content("请添加参数：orderId");
            var url = "https://waimaiopen.meituan.com/api/v1/order/refund/agree";
            var mt = new MTInputData(Business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", Business.MT_AppId);
            mt.SetValue("order_id", orderId);
            mt.SetValue("reason", "用户不想要了");
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            var content = await UtilHelper.RequestAsync(url, method: "get");
            return Content(content);
        }

        /// <summary>
        /// 驳回订单退款申请
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RefundReject([FromQuery]string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return Content("请添加参数：orderId");
            var url = "https://waimaiopen.meituan.com/api/v1/order/refund/reject";
            var mt = new MTInputData(Business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", Business.MT_AppId);
            mt.SetValue("order_id", orderId);
            mt.SetValue("reason", "用户不想要了");
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            var content = await UtilHelper.RequestAsync(url, method: "get");
            return Content(content);
        }

        /// <summary>
        /// 拉取用户真实手机号
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetUserPhones()
        {
            var url = "https://waimaiopen.meituan.com/api/v1/order/batchPullPhoneNumber";
            var mt = new MTInputData(Business.MT_AppKey, url);
            mt.SetValue("timestamp", DateTime.Now.ToTimestamp());
            mt.SetValue("app_id", Business.MT_AppId);
            mt.SetValue("offset", 0);
            mt.SetValue("limit", 100);
            var sig = mt.MakeSign();
            mt.SetValue("sig", sig);
            url = $"{url}?{mt.ToUrl()}";
            var content = await UtilHelper.RequestAsync(url, method: "post");
            return Content(content);
        }

        #endregion


    }
}