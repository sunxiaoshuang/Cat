using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JdCat.Cat.Web.App_Code
{
    /// <summary>
    /// 点我达接口请求帮助类
    /// </summary>
    public class DwdHelper
    {
        private string _enviroment;
        public string AppKey { get; private set; }
        public string Secret { get; private set; }
        public string Domain { get; private set; }

        public DwdHelper(AppData data)
        {
            AppKey = data.DwdAppKey;
            Secret = data.DwdAppSecret;
            Domain = data.DwdDomain;
            _enviroment = data.RunMode;
        }

        /// <summary>
        /// 请求通用接口
        /// </summary>
        /// <param name="api">接口</param>
        /// <param name="sign">签名</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public async Task<DWD_Result<T>> RequestAsync<T>(string api, string sign, string data) where T : class, new()
        {
            var trans = new DWD_Trans { pk = AppKey, timestamp = UtilHelper.ConvertDateTimeToInt(DateTime.Now) };
            trans.sig = UtilHelper.SHA1(Secret + sign + Secret);
            trans.Generate();

            using (var client = new HttpClient())
            {
                var content = new StringContent(trans.Params() + "&" + data);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = await client.PostAsync(Domain + api, content);
                var result = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DWD_Result<T>>(result);
            }
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="business">应该传入点我达商户</param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_Content>> SendOrderAsync(Order order, Business business)
        {
            order.DistributionFlow++;
            var now = UtilHelper.ConvertDateTimeToInt(DateTime.Now);
            var dwdOrder = new DWD_Order
            {
                order_original_id = GetOrderCode(order),
                order_create_time = now,
                order_remark = order.Remark,
                order_price = (int)order.Price.Value * 100,
                serial_id = order.Identifier + "",
                cargo_weight = 0,
                city_code = business.DwdShop.city_code,
                seller_id = business.DwdShop.external_shopid,
                seller_name = business.Name,
                seller_mobile = business.Mobile,
                seller_address = business.Address,
                seller_lat = business.Lat,
                seller_lng = business.Lng,
                consignee_name = order.ReceiverName,
                consignee_mobile = order.Phone,
                consignee_address = order.ReceiverAddress,
                consignee_lat = order.Lat,
                consignee_lng = order.Lng,
                delivery_fee_from_seller = 0                 // 需要修改
            };
            if(_enviroment == "test")
            {
                dwdOrder.seller_lng = 120.165993;
                dwdOrder.seller_lat = 30.315408;
                dwdOrder.consignee_lng = 120.168513;
                dwdOrder.consignee_lat = 30.315272;
            }
            var products = order.Products.Select(a => new DWD_Product { item_name = a.Name, discount_price = (int)(a.Price.Value * 100), production_time = 0, quantity = (int)a.Quantity.Value, unit = "份", unit_price = (int)(a.OldPrice.Value * 100) });
            dwdOrder.items = JsonConvert.SerializeObject(products);
            dwdOrder.Generate();

            return await RequestAsync<DWD_Content>("/api/v3/order-send.json", dwdOrder.Sign(), dwdOrder.Params());
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public async Task<DWD_Result<JsonData>> CancelOrderAsync(Order order, string reason)
        {
            var url = "/api/v3/order-cancel.json";
            var sign = $"cancle_reason{reason}order_original_id{GetOrderCode(order)}";
            var data = $"cancle_reason={reason}&order_original_id={GetOrderCode(order)}";
            return await RequestAsync<JsonData>(url, sign, data);
        }

        public async Task<DWD_Result<JsonData>> CreateShop(DWD_Business business)
        {
            var url = "/api/v3/batchsave-store.json";
            var shops = new List<DWD_Shop> { new DWD_Shop { addr = business.addr, city_code = business.city_code, external_shopid = business.external_shopid, lat = business.lat, lng = business.lng, mobile = business.mobile, shop_title = business.shop_title } };
            var json = JsonConvert.SerializeObject(shops);
            var sign = $"shops{json}";
            var data = $"shops={json}";
            return await RequestAsync<JsonData>(url, sign, data);
        }

        public async Task<DWD_Result<DWD_Balance>> GetBalance(string shopId)
        {
            var url = "/api/v3/account-balance.json";
            var sign = $"account_typestorestore_id{shopId}";
            var data = $"account_type=store&store_id={shopId}";
            return await RequestAsync<DWD_Balance>(url, sign, data);
        }

        private string GetOrderCode(Order order)
        {
            return order.OrderCode + "_" + order.DistributionFlow;
        }
    }
}
