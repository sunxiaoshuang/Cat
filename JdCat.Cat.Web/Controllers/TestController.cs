﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository.Model;
using JdCat.Cat.Repository.Service;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        //public IActionResult Decrypted([FromServices]UtilHelper util)
        //{
        //    var encryptedData = "KB2Wfoq7RKPjGsX3z8uO2XTzKbVqw7Sso/vzFlrLWmchnvHt21+SfEUUchwd1408XBNGhzSCDh7yyTu+9a5tgCxrU8nzOHji7o8k1ZjNiaV7DrZOEtzvOZmQUuDk16ozJtXb11C1slU5ATwXwtwEMGwmGg1RaJ6ux7GH4AJeAwHtcJjAaqbJQ2s6j8iKUM3BmP2/VDCr/Ioh2n6LJS37ww==";
        //    var sessionKey = "Vb9X2sRS8++jD/q9MPRPDw==";
        //    var appId = "wx7fc7dac038048c37";

        //    var iv = "QGtlrCecZJtuEc1HaVI4Eg==";
        //    var result = util.AESDecrypt(encryptedData, sessionKey, iv);
        //    var exist = result.Contains(appId);
        //    var user = JsonConvert.DeserializeObject<WxUser>(result);
        //    return Ok(result + "||appid:" + appId);
        //}

        public IActionResult Video()
        {
            return View();
        }

        public IActionResult WsService()
        {
            return View();
        }

        public IActionResult AppData()
        {
            var appData = HttpContext.RequestServices.GetService<AppData>();
            return Ok(JsonConvert.SerializeObject(appData));
        }

        public IActionResult CityList([FromServices]List<City> list, [FromServices]JsonSerializerSettings setting)
        {
            return Json(list, setting);
        }

        public async Task<IActionResult> Print([FromServices]FeYinHelper helper, [FromServices]IOrderRepository service)
        {
            var order = service.Set<Order>().Include(a => a.Products).SingleOrDefault(a => a.ID == 5);
            var result = await helper.Print("4600416530039455", order);
            return Ok(result.ErrMsg ?? "正在打印中，请稍等");
        }

        public async Task<IActionResult> UpdateToken([FromServices]FeYinHelper helper)
        {
            await helper.GetToken();
            return Ok("成功");
        }

        public async Task<IActionResult> BindDevice([FromServices]FeYinHelper helper, [FromQuery]string device_no)
        {
            var result = await helper.BindDevice(device_no);
            return Ok(result.ErrMsg ?? "设备绑定成功");
        }
        public async Task<IActionResult> UnBindDevice([FromServices]FeYinHelper helper, [FromQuery]string device_no)
        {
            var result = await helper.UnBindDevice(device_no);
            return Ok(result.ErrMsg ?? "设备解绑成功");
        }

        public IActionResult LazyLoad([FromServices]IBusinessRepository service)
        {
            var business = service.Get(1);
            return Content(business.Orders.First().CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        #region 达达

        public IActionResult DadaCallback([FromBody]DadaCallBack dada, [FromServices]IHostingEnvironment environment)
        {
            var filename = Path.Combine(environment.ContentRootPath, "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            System.IO.File.AppendAllText(filename, "\r\n" + Environment.NewLine + JsonConvert.SerializeObject(dada));
            return Ok("成功");
        }

        public IActionResult DadaTest()
        {
            return View();
        }

        /// <summary>
        /// 达达模拟接单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> DadaReceive([FromQuery]string orderCode)
        {
            return Ok(await DadaSimulation(orderCode, "/api/order/accept"));
        }
        /// <summary>
        /// 达达模拟取货
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> DadaFetch([FromQuery]string orderCode)
        {
            return Ok(await DadaSimulation(orderCode, "/api/order/fetch"));
        }
        /// <summary>
        /// 达达模拟完成订单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> DadaFinish([FromQuery]string orderCode)
        {
            return Ok(await DadaSimulation(orderCode, "/api/order/finish"));
        }
        /// <summary>
        /// 达达模拟取消订单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> DadaCancel([FromQuery]string orderCode)
        {
            return Ok(await DadaSimulation(orderCode, "/api/order/cancel", "取消订单"));
        }
        /// <summary>
        /// 达达模拟订单过期
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<IActionResult> DadaExpire([FromQuery]string orderCode)
        {
            return Ok(await DadaSimulation(orderCode, "/api/order/expire"));
        }
        /// <summary>
        /// 模拟达达
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> DadaSimulation(string orderCode, string url, string msg = null)
        {
            var appData = HttpContext.RequestServices.GetService<AppData>();
            var trans = new DadaTrans { Timestamp = UtilHelper.ConvertDateTimeToInt(DateTime.Now) };
            trans.App_key = appData.DadaAppKey;
            trans.App_secret = appData.DadaAppSecret;
            trans.Source_id = appData.DadaSourceId;
            trans.Body = JsonConvert.SerializeObject(new { order_id = orderCode, reason = msg });
            trans.Generator();
            using (var hc = new HttpClient())
            {
                var p = JsonConvert.SerializeObject(trans, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var body = new StringContent(p);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await hc.PostAsync(appData.DadaDomain + url, body);
                return await result.Content.ReadAsStringAsync();
            }
        }
        /// <summary>
        /// 达达获取取消原因
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DadaCancelReason()
        {
            using (var hc = new HttpClient())
            {
                var appData = HttpContext.RequestServices.GetService<AppData>();
                var trans = new DadaTrans { Timestamp = UtilHelper.ConvertDateTimeToInt(DateTime.Now) };
                trans.App_key = appData.DadaAppKey;
                trans.App_secret = appData.DadaAppSecret;
                trans.Source_id = appData.DadaSourceId;
                trans.Body = "";
                trans.Generator();
                var p = JsonConvert.SerializeObject(trans, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var body = new StringContent(p);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await hc.PostAsync(appData.DadaDomain + "/api/order/cancel/reasons", body);
                return Ok(await result.Content.ReadAsStringAsync());
            }
        }
        /// <summary>
        /// 达达获取城市列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DadaCityList()
        {
            using (var hc = new HttpClient())
            {
                var appData = HttpContext.RequestServices.GetService<AppData>();
                var trans = new DadaTrans { Timestamp = UtilHelper.ConvertDateTimeToInt(DateTime.Now) };
                trans.App_key = appData.DadaAppKey;
                trans.App_secret = appData.DadaAppSecret;
                trans.Source_id = appData.DadaSourceId;
                trans.Body = "";
                trans.Generator();
                var p = JsonConvert.SerializeObject(trans, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var body = new StringContent(p);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await hc.PostAsync(appData.DadaDomain + "/api/cityCode/list", body);
                return Ok(await result.Content.ReadAsStringAsync());
            }
        }

        #endregion
    }
}