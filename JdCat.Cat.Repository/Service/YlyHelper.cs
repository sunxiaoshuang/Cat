using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.Repository.Service
{
    public class YlyHelper
    {
        private string partner;
        private string apikey;
        private string url;

        private static YlyHelper _helper;
        static YlyHelper()
        {
            _helper = new YlyHelper();
        }
        public static YlyHelper GetHelper()
        {
            return _helper;
        }
        private YlyHelper()
        {
        }
        public void Init(string partnerId, string apikey, string url)
        {
            this.partner = partnerId;
            this.apikey = apikey;
            this.url = url;
        }

        /// <summary>
        /// 打印小票
        /// </summary>
        /// <param name="order"></param>
        /// <param name="device"></param>
        /// <param name="business"></param>
        /// <returns></returns>
        public async Task<string> PrintAsync(Order order, FeyinDevice device, Business business)
        {
            var time = DateTime.Now.ToTimestamp().ToString();
            var dic = new Dictionary<string, string>
            {
                { "machine_code", device.Code },
                { "partner", partner },
                { "time", time }
            };
            var sign = GetSign(dic, apikey, device.ApiKey).ToUpper();
            var content = new StringBuilder();
            content.Append("<MS>1,3</MS>");
            content.Append($"<center><FS2>#{order.Identifier} 简单猫</FS2></center>\r\n");
            content.Append("配送小票\r\n");
            content.Append("--------------------------------\r\n");
            if (!string.IsNullOrEmpty(order.Remark))
            {
                content.Append($"<FS2>备注：{order.Remark}</FS2>\r\n");
            }
            content.Append($"<center><FS2>{business.Name}</FS2></center>\r\n");
            content.Append($"下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}\r\n");
            content.Append($"订单编号：{order.OrderCode}\r\n");
            content.Append("--------------------------------");
            if (order.Products == null || order.Products.Count == 0)
            {
                content.Append("无任何商品\r\n");
            }
            else
            {
                content.Append("<table>");
                foreach (var item in order.Products)
                {
                    content.Append($"<tr><td>{item.Name}</td><td>* {item.Quantity}</td><td>{item.Price}</td></tr>");
                }
                content.Append("</table>");
            }
            content.Append("--------------------------------");
            if (order.PackagePrice.HasValue)
            {
                content.Append($"{UtilHelper.PrintLineLeftRight("包装费", Convert.ToDouble(order.PackagePrice.Value) + "")}\n");
            }
            content.Append($"{UtilHelper.PrintLineLeftRight("配送费", Convert.ToDouble(order.Freight.Value) + "")}\n");
            if (order.SaleCouponUser != null)
            {
                content.Append($"{UtilHelper.PrintLineLeftRight("[" + order.SaleCouponUser.Name + "]", "-￥" + Convert.ToDouble(order.SaleCouponUser.Value) + "")}\n");
            }
            if (order.SaleFullReduce != null)
            {
                content.Append($"{UtilHelper.PrintLineLeftRight("[" + order.SaleFullReduce.Name + "]", "-￥" + Convert.ToDouble(order.SaleFullReduce.ReduceMoney) + "")}\n");
            }
            content.Append("--------------------------------");
            if (order.OldPrice.HasValue && order.Price != order.OldPrice)
            {
                content.Append($"<right>原价：{order.OldPrice.Value}元</right>");
            }
            content.Append($"<right>实付：{order.Price}元</right>\r\n");
            content.Append("--------------------------------");
            content.Append($"<FS2>{order.ReceiverAddress}</FS2>\r\n");
            content.Append($"<FS2>{order.Phone}</FS2>\r\n");
            content.Append($"<FS2>{order.GetUserCall()}</FS2>\r\n");
            if (!string.IsNullOrEmpty(business.AppQrCode))
            {
                content.Append($"<QR>{business.AppQrCode}</QR>");
            }
            content.Append("--------------------------------");
            content.Append($"<center><FS2>#{order.Identifier}完</FS2></center>");

            var trans = $"partner={partner}&machine_code={device.Code}&time={time}&sign={sign}&content={content}";
            using (var client = new HttpClient())
            {
                var body = new StringContent(trans);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                return result;
            }
        }
        /// <summary>
        /// 打印提示信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="device_no"></param>
        /// <returns></returns>
        public async Task<string> PrintAsync(string content, FeyinDevice device)
        {
            var str = string.Empty;
            str += "**************通知**************\r\n";
            str += "<FS2>简单猫提示您：</FS2>\r\n" + $"<FS2>{content}</FS2>\r\n\r\n";
            str += "********************************\r\n";
            var time = DateTime.Now.ToTimestamp().ToString();
            var dic = new Dictionary<string, string>
            {
                { "machine_code", device.Code },
                { "partner", partner },
                { "time", time }
            };
            var sign = GetSign(dic, apikey, device.ApiKey).ToUpper();
            var trans = $"partner={partner}&machine_code={device.Code}&time={time}&sign={sign}&content={str}";
            using (var client = new HttpClient())
            {
                var body = new StringContent(trans);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                return result;
            }
        }

        private string GetSign(IDictionary<string, string> parameters, string apikey, string msign)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters);
            var dem = sortedParams.GetEnumerator();
            var query = new StringBuilder();
            while (dem.MoveNext())
            {
                var key = dem.Current.Key;
                var value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            var source = query.ToString();
            source = apikey + source + msign;
            return UtilHelper.MD5Encrypt(source);
        }


    }
}
