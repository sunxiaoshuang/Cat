﻿using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository.Model;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Repository.Service
{
    /// <summary>
    /// 达达请求接口
    /// </summary>
    public class DadaHelper
    {
        private static DadaHelper helper;
        static DadaHelper()
        {
            helper = new DadaHelper();
        }
        protected DadaHelper()
        {

        }
        public static DadaHelper GetHelper() => helper;

        private AppData _appData;
        private JsonSerializerSettings _setting;
        private string _domain;
        private static readonly ILog _log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(DadaHelper));
        public ILog Log => _log;
        public void Init(AppData appData, JsonSerializerSettings setting)
        {
            this._appData = appData;
            this._domain = _appData.DadaDomain;
            this._setting = setting;
        }

        /// <summary>
        /// 达达请求通用接口
        /// </summary>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <param name="app_key"></param>
        /// <param name="app_secret"></param>
        /// <param name="source_id"></param>
        /// <returns></returns>
        public async Task<string> RequestAsync(string api, object data = null, string source_id = null)
        {
            using (var hc = new HttpClient())
            {
                var req = new DadaTrans { Timestamp = UtilHelper.ConvertDateTimeToInt(DateTime.Now) };
                req.App_key = _appData.DadaAppKey;
                req.App_secret = _appData.DadaAppSecret;
                if (_appData.RunMode == "test")
                {
                    req.Source_id = _appData.DadaSourceId;
                }
                else
                {
                    req.Source_id = source_id ?? _appData.DadaSourceId;
                }
                if (data != null)
                {
                    req.Body = JsonConvert.SerializeObject(data, _setting);
                }
                req.Generator();
                var p = JsonConvert.SerializeObject(req, _setting);
                var body = new StringContent(p);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await hc.PostAsync(this._domain + api, body);
                return await result.Content.ReadAsStringAsync();
            }
        }
        /// <summary>
        /// 达达请求通用接口
        /// </summary>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <param name="app_key"></param>
        /// <param name="app_secret"></param>
        /// <param name="source_id"></param>
        /// <returns></returns>
        public async Task<DadaResult<T>> RequestAsync<T>(string api, object data = null, string source_id = null) where T: class, new()
        {
            var result = await RequestAsync(api, data, source_id);
            return JsonConvert.DeserializeObject<DadaResult<T>>(result);
        }

        /// <summary>
        /// 创建达达订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<DadaResult<DadaReturn>> SendOrderAsync(Order order, Business business)
        {
            var dadaOrder = new DadaOrder();
            if(_appData.RunMode == "test")
            {
                dadaOrder.shop_no = _appData.DadaShopNo;
            }
            else
            {
                dadaOrder.shop_no = business.DadaShopNo;
            }
            dadaOrder.origin_id = order.OrderCode;
            dadaOrder.city_code = order.CityCode;
            dadaOrder.cargo_price = (double)order.Price;
            dadaOrder.receiver_name = order.GetUserCall();
            dadaOrder.receiver_address = order.ReceiverAddress;
            dadaOrder.receiver_lat = order.Lat;
            dadaOrder.receiver_lng = order.Lng;
            dadaOrder.callback = _appData.DadaCallback;
            dadaOrder.receiver_phone = order.Phone;
            dadaOrder.origin_mark_no = order.Identifier.ToString();
            var url = order.IsSendDada ? "/api/order/reAddOrder" : "/api/order/addOrder";
            return await RequestAsync<DadaReturn>(url, dadaOrder, business.DadaSourceId);
        }

        /// <summary>
        /// 取消达达订单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<DadaResult<DadaLiquidatedDamages>> CancelOrderAsync(string orderCode, Business business, int reasonId, string reason)
        {
            return await RequestAsync<DadaLiquidatedDamages>("/api/order/formalCancel", new { order_id = orderCode, cancel_reason_id = reasonId, cancel_reason = reason }, business.DadaSourceId);
        }
        /// <summary>
        /// 为达达订单添加小费
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="tips"></param>
        /// <param name="cityCode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        //public async Task<string> AddTip(string orderCode, double tips, string cityCode, string info)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
