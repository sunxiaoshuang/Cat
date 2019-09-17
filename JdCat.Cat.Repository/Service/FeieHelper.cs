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
using JdCat.Cat.Repository.Model;

namespace JdCat.Cat.Repository.Service
{
    public class FeieHelper
    {
        private string user;
        private string key;
        private string url;

        private static FeieHelper _helper;
        static FeieHelper()
        {
            _helper = new FeieHelper();
        }
        public static FeieHelper GetHelper()
        {
            return _helper;
        }
        private FeieHelper()
        {
        }
        public void Init(string user, string key, string url)
        {
            this.user = user;
            this.key = key;
            this.url = url;
        }

        /// <summary>
        /// 添加打印机
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<FeieReturn> AddPrintAsync(FeyinDevice device)
        {
            var time = DateTime.Now.ToTimestamp();
            var sign = UtilHelper.SHA1(user + key + time).ToLower();

            var postData = $"printerContent={device.Code}# {device.ApiKey}# {device.Name} #{device.Remark}";
            postData += ("&user=" + user);
            postData += ("&stime=" + time);
            postData += ("&sig=" + sign);
            postData += ("&apiname=Open_printerAddlist");

            using (var client = new HttpClient())
            {
                var body = new StringContent(postData);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var result = await client.PostAsync(url, body);
                return JsonConvert.DeserializeObject<FeieReturn>(await result.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// 打印订单小票
        /// </summary>
        /// <param name="order"></param>
        /// <param name="device"></param>
        /// <param name="business"></param>
        /// <returns></returns>
        public async Task<string> PrintAsync(Order order, FeyinDevice device, Business business)
        {
            var time = DateTime.Now.ToTimestamp().ToString();
            var sign = UtilHelper.SHA1(user + key + time).ToLower();
            var content = new StringBuilder();
            content.Append($"<CB>#{order.Identifier} 简单猫</CB>");

            content.Append("配送小票<BR>");
            content.Append("--------------------------------<BR>");
            if (!string.IsNullOrEmpty(order.Remark))
            {
                content.Append($"<B>备注：{order.Remark}</B><BR>");
            }
            content.Append($"<CB>{business.Name}</CB><BR>");
            content.Append($"下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}<BR>");
            content.Append($"订单编号：{order.OrderCode}<BR>");
            content.Append("--------------------------------");
            if (order.Products == null || order.Products.Count == 0)
            {
                content.Append("无任何商品<BR>");
            }
            else
            {
                // 商品名称占用打印纸的20个字符位
                var position = 20;
                var other = 6;
                foreach (var product in order.Products)
                {
                    // 商品名称
                    var name = product.Name;
                    var zhQuantity = 0;              // 中文字符数
                    var enQuantity = 0;              // 其他字符数
                    var cutName = string.Empty;      // 截取的名称
                    while (true)
                    {
                        zhQuantity = UtilHelper.CalcZhQuantity(name);
                        enQuantity = name.Length - zhQuantity;
                        if (zhQuantity * 2 + enQuantity > position)
                        {
                            cutName += name.Substring(name.Length - 2);
                            name = name.Substring(0, name.Length - 2);
                        }
                        else
                        {
                            break;
                        }
                    }
                    var nameLen = zhQuantity * 2 + enQuantity;
                    for (int i = 0; i < position - nameLen; i++)
                    {
                        name += " ";
                    }
                    // 商品数量
                    var quantity = "* " + Convert.ToDouble(product.Quantity);
                    var quantityLen = quantity.Length;
                    for (int i = 0; i < other - quantityLen; i++)
                    {
                        quantity += " ";
                    }
                    // 商品价格
                    var price = Convert.ToDouble(product.Price.Value) + "";
                    var priceLen = price.Length;
                    for (int i = 0; i < other - priceLen; i++)
                    {
                        price = " " + price;
                    }
                    content.Append($"{name + quantity + price}<BR>");
                    if (!string.IsNullOrEmpty(cutName))
                    {
                        content.Append($"{cutName}<BR>");
                    }
                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        content.Append($"（{product.Description}）<BR>");
                    }
                }
            }
            content.Append("--------------其他--------------<BR>");
            if (order.PackagePrice.HasValue)
            {
                content.Append($"{UtilHelper.PrintLineLeftRight("包装费", Convert.ToDouble(order.PackagePrice.Value) + "")}<BR>");
            }
            content.Append($"{UtilHelper.PrintLineLeftRight("配送费", Convert.ToDouble(order.Freight.Value) + "")}<BR>");
            //if (order.SaleCouponUser != null)
            //{
            //    content.Append($"{UtilHelper.PrintLineLeftRight("[" + order.SaleCouponUser.Name + "]", "-￥" + Convert.ToDouble(order.SaleCouponUser.Value) + "")}<BR>");
            //}
            //if (order.SaleFullReduce != null)
            //{
            //    content.Append($"{UtilHelper.PrintLineLeftRight("[" + order.SaleFullReduce.Name + "]", "-￥" + Convert.ToDouble(order.SaleFullReduce.ReduceMoney) + "")}<BR>");
            //}
            if (order.OrderActivities != null && order.OrderActivities.Count > 0)
            {
                order.OrderActivities.ForEach(a => content.Append($"{UtilHelper.PrintLineLeftRight(a.Remark, a.Amount + "")}<BR>"));
            }
            content.Append("--------------------------------<BR>");
            if (order.OldPrice.HasValue && order.Price != order.OldPrice)
            {
                content.Append($"<RIGHT>原价：{order.OldPrice.Value}元</RIGHT><BR>");
            }
            content.Append($"<RIGHT>实付：<B>{order.Price}元</B></RIGHT><BR>");
            content.Append("--------------------------------<BR>");
            content.Append($"<B>{order.ReceiverAddress}</B><BR>");
            content.Append($"<B>{order.Phone}</B><BR>");
            content.Append($"<B>{order.GetUserCall()}</B><BR>");
            if (!string.IsNullOrEmpty(business.AppQrCode))
            {
                content.Append($"<QR>{business.AppQrCode}</QR>");
            }
            content.Append($"**********<B>#{order.Identifier}</B>完**********<BR>");
            var orderInfo = content.ToString().ToUrlEncoding();

            string postData = "sn=" + device.Code;
            postData += ("&content=" + orderInfo);
            postData += ("&times=" + "1");//默认1联
            //公共参数
            postData += ("&user=" + user);
            postData += ("&stime=" + time);
            postData += ("&sig=" + sign);
            postData += ("&apiname=" + "Open_printMsg");

            using (var client = new HttpClient())
            {
                var body = new StringContent(postData);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var result = await client.PostAsync(url, body);
                return await result.Content.ReadAsStringAsync();
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
            str += "**************通知**************<BR>";
            str += "<B>简单猫提示您：</B><BR>" + $"<B>{content}</B><BR><BR>";
            str += "********************************<BR>";

            var time = DateTime.Now.ToTimestamp().ToString();
            var sign = UtilHelper.SHA1(user + key + time).ToLower();

            string postData = "sn=" + device.Code;
            postData += ("&content=" + str.ToUrlEncoding());
            postData += ("&times=" + "1");
            postData += ("&user=" + user);
            postData += ("&stime=" + time);
            postData += ("&sig=" + sign);
            postData += ("&apiname=" + "Open_printMsg");

            using (var client = new HttpClient())
            {
                var body = new StringContent(postData);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var result = await client.PostAsync(url, body);
                return await result.Content.ReadAsStringAsync();
            }
        }


    }
}
