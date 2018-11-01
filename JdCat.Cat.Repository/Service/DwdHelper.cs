using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JdCat.Cat.Repository.Service
{
    /// <summary>
    /// 点我达接口请求帮助类
    /// </summary>
    public class DwdHelper
    {
        private static DwdHelper helper;
        public static Dictionary<DWD_OrderStatus, string> DwdStatusDic;
        static DwdHelper()
        {
            DwdStatusDic = new Dictionary<DWD_OrderStatus, string> {
                { DWD_OrderStatus.Assigning, "派单中" },
                { DWD_OrderStatus.Transfer, "已转单" },
                { DWD_OrderStatus.Taking, "取餐中" },
                { DWD_OrderStatus.ArrivedShop, "已到店" },
                { DWD_OrderStatus.Distribution, "配送中" },
                { DWD_OrderStatus.Finish, "已完成" },
                { DWD_OrderStatus.Exception, "异常" },
                { DWD_OrderStatus.Cancel, "已取消" }
            };
            helper = new DwdHelper();
        }
        protected DwdHelper()
        {

        }
        public static DwdHelper GetHelper() => helper;
        private string _enviroment;
        public string AppKey { get; private set; }
        public string Secret { get; private set; }
        public string Domain { get; private set; }

        public void Init(AppData data)
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
        /// 预估费用
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="address"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_CostResult>> CostAsync(double lng, double lat, string address, DWDStore store)
        {
            var cost = new DWD_Cost
            {
                city_code = store.city_code,
                seller_id = store.external_shopid,
                seller_name = store.shop_title,
                seller_mobile = store.mobile,
                seller_address = store.addr,
                seller_lat = store.lat,
                seller_lng = store.lng,
                consignee_address = address,
                consignee_lat = lat,
                consignee_lng = lng
            };
            cost.Generate();
            return await RequestAsync<DWD_CostResult>("/api/v3/cost-estimate.json", cost.Sign(), cost.Params());
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
                order_remark = (order.Remark + "").ToEncodeSpecial(),
                order_price = (int)order.Price.Value * 100,
                serial_id = order.Identifier + "",
                cargo_weight = 0,
                city_code = business.DWDStore.city_code,
                seller_id = business.DWDStore.external_shopid,
                seller_name = business.Name,
                seller_mobile = business.Mobile,
                seller_address = business.Address.ToEncodeSpecial(),
                seller_lat = business.DWDStore.lat,
                seller_lng = business.DWDStore.lng,
                consignee_name = order.ReceiverName.ToEncodeSpecial(),
                consignee_mobile = order.Phone.ToEncodeSpecial(),
                consignee_address = order.ReceiverAddress.ToEncodeSpecial(),
                consignee_lat = order.Lat,
                consignee_lng = order.Lng,
                delivery_fee_from_seller = 10                 // 需要修改
            };
            if (_enviroment == "test")
            {
                dwdOrder.city_code = "330100";
                dwdOrder.seller_lng = 120.165993;
                dwdOrder.seller_lat = 30.315408;
                dwdOrder.consignee_lng = 120.168513;
                dwdOrder.consignee_lat = 30.315272;
            }
            var products = order.Products.Select(a => new DWD_Product { item_name = a.Name, discount_price = (int)(a.Price.Value * 100), production_time = 0, quantity = (int)a.Quantity.Value, unit = "份", unit_price = (int)(a.OldPrice == null ? 0 : a.OldPrice.Value * 100) });
            dwdOrder.items = JsonConvert.SerializeObject(products).ToEncodeSpecial();
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

        /// <summary>
        /// 创建商户
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        public async Task<DWD_Result<JsonData>> CreateShop(DWDStore business)
        {
            var url = "/api/v3/batchsave-store.json";
            var shops = new List<DWD_Shop> { new DWD_Shop { addr = business.addr, city_code = business.city_code, external_shopid = business.external_shopid, lat = business.lat, lng = business.lng, mobile = business.mobile, shop_title = business.shop_title } };
            var json = JsonConvert.SerializeObject(shops);
            var sign = $"shops{json}";
            var data = $"shops={json}";
            return await RequestAsync<JsonData>(url, sign, data);
        }

        /// <summary>
        /// 获取商户余额
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_Balance>> GetBalance(string shopId)
        {
            var url = "/api/v3/account-balance.json";
            var sign = $"account_typestorestore_id{shopId}";
            var data = $"account_type=store&store_id={shopId}";
            return await RequestAsync<DWD_Balance>(url, sign, data);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_RechargeBack>> Recharge(DWD_Recharge model)
        {
            var url = "/api/v3/recharge.json";
            var obj = new DWD_Amout
            {
                amout = long.Parse((model.Amount * 100).ToString()),
                rechange_channle = model.Mode.ToString().ToLower(),
                return_url = "",
                serial_no = model.Code,
                store_id = model.DWD_Business.external_shopid
            };
            obj.Generate();
            var sign = obj.Sign();
            var data = obj.Params();
            return await RequestAsync<DWD_RechargeBack>(url, sign, data);
        }

        /// <summary>
        /// 查看充值结果
        /// </summary>
        /// <param name="store"></param>
        /// <param name="biz_no"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_RechargeResult>> RechargeResult(DWDStore store, string biz_no)
        {
            var url = "/api/v3/recharge-result.json";
            var sign = $"biz_no{biz_no}store_id{store.external_shopid}";
            var data = $"biz_no={biz_no}&store_id={store.external_shopid}";
            return await RequestAsync<DWD_RechargeResult>(url, sign, data);
        }

        /// <summary>
        /// 获取交易列表
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_Detail>> GetDetail(DWDStore store, DateTime startDate, DateTime endDate, int pageIndex)
        {
            var url = "/api/v3/get-bill-detail.json";
            var start = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            endDate = endDate.AddDays(1).AddSeconds(-1);
            var end = endDate.ToString("yyyy-MM-dd HH:mm:ss");
            var sign = $"end_time{end}page{pageIndex}start_time{start}store_id{store.external_shopid}";
            var data = $"end_time={end}&page={pageIndex}&start_time={start}&store_id={store.external_shopid}";
            return await RequestAsync<DWD_Detail>(url, sign, data);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_OrderDetail>> GetOrderDetail(string code)
        {
            var url = "/api/v3/order-get.json";
            var sign = $"order_original_id{code}";
            var data = $"order_original_id={code}";
            return await RequestAsync<DWD_OrderDetail>(url, sign, data);
        }
        
        /// <summary>
        /// 获取订单运费
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<DWD_Result<DWD_Price>> GetOrderPrice(string code)
        {
            var url = "/api/v3/order-receivable-price.json";
            var sign = $"order_original_id{code}";
            var data = $"order_original_id={code}";
            return await RequestAsync<DWD_Price>(url, sign, data);
        }

        #region 私有方法

        public string GetOrderCode(Order order)
        {
            return order.OrderCode + "_" + order.DistributionFlow;
        }

        #endregion

    }
}
