using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Repository.Service
{
    public class WmgjHelper
    {
        private int app_id;
        private string app_key;
        private string url;
        private const string pwd = "888888";

        private static WmgjHelper _helper;
        static WmgjHelper()
        {
            _helper = new WmgjHelper();
        }
        public static WmgjHelper GetHelper()
        {
            return _helper;
        }
        private WmgjHelper()
        {
        }
        public void Init(int app_id, string app_key, string url)
        {
            this.app_id = app_id;
            this.app_key = app_key;
            this.url = url;
        }

        /// <summary>
        /// 添加打印机
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<WmgjReturn<WmgjNewPrinter>> AddPrintAsync(FeyinDevice device)
        {
            var authUrl = "http://open.xiaowm.com/Auth_api/auth";
            var content = new {
                number = device.Code,
                app_id,
                pwd
            };

            using (var client = new HttpClient())
            {
                var body = new StringContent(JsonConvert.SerializeObject(content));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await client.PostAsync(authUrl, body);
                return JsonConvert.DeserializeObject<WmgjReturn<WmgjNewPrinter>>(await result.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public async Task<string> DelPrinterAsync(FeyinDevice device)
        {
            var delUrl = "http://open.xiaowm.com/Auth_api/delete";
            var content = new {
                number = device.Code,
                token = device.Remark
            };
            using (var client = new HttpClient())
            {
                var body = new StringContent(JsonConvert.SerializeObject(content));
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var ret = await client.PostAsync(delUrl, body);
                var result = await ret.Content.ReadAsStringAsync();
                return result;
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
            var content = new StringBuilder();
            content.Append($"|8  #{order.Identifier} 简单猫");

            content.Append("|6配送小票");
            content.Append("|6------------------------");
            if (!string.IsNullOrEmpty(order.Remark))
            {
                content.Append($"|8备注：{order.Remark}");
            }
            content.Append($"|7  {business.Name}");
            content.Append($"|4下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}");
            content.Append($"|4订单编号：{order.OrderCode}");
            content.Append("|6------------------------");
            if (order.Products == null || order.Products.Count == 0)
            {
                content.Append("|6无任何商品");
            }
            else
            {
                // 商品名称占用打印纸的16个字符位
                var position = 16;
                var other = 4;
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
                    var quantity = "*" + product.Quantity;
                    var quantityLen = quantity.Length;
                    for (int i = 0; i < other - quantityLen; i++)
                    {
                        quantity += " ";
                    }
                    // 商品价格
                    var price = product.Price.Value + "";
                    var priceLen = price.Length;
                    for (int i = 0; i < other - priceLen; i++)
                    {
                        price = " " + price;
                    }
                    content.Append($"|6{name + quantity + price}");
                    if (!string.IsNullOrEmpty(cutName))
                    {
                        content.Append($"|6{cutName}");
                    }
                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        content.Append($"|6（{product.Description}）");
                    }
                }
            }
            content.Append("|6----------其他----------");
            if (order.PackagePrice.HasValue)
            {
                content.Append($"|6{UtilHelper.PrintLineLeftRight("包装费", Convert.ToDouble(order.PackagePrice.Value) + "", 24)}");
            }
            content.Append($"|6{UtilHelper.PrintLineLeftRight("配送费", Convert.ToDouble(order.Freight.Value) + "", 24)}");
            //if (order.SaleCouponUser != null)
            //{
            //    content.Append($"|6{UtilHelper.PrintLineLeftRight("[" + order.SaleCouponUser.Name + "]", "-￥" + Convert.ToDouble(order.SaleCouponUser.Value) + "", 24)}");
            //}
            //if (order.SaleFullReduce != null)
            //{
            //    content.Append($"|6{UtilHelper.PrintLineLeftRight("[" + order.SaleFullReduce.Name + "]", "-￥" + Convert.ToDouble(order.SaleFullReduce.ReduceMoney) + "", 24)}");
            //}
            if (order.OrderActivities != null && order.OrderActivities.Count > 0)
            {
                order.OrderActivities.ForEach(a => content.Append($"|6{UtilHelper.PrintLineLeftRight("[" + a.Remark + "]", "-￥" + a.Amount + "", 24)}"));
            }
            content.Append("|6------------------------");
            if (order.OldPrice.HasValue && order.Price != order.OldPrice)
            {
                content.Append($"|6原价：{order.OldPrice}元");
            }
            content.Append($"|6实付：{order.Price}元");
            content.Append("|6------------------------");
            content.Append($"|8{order.ReceiverAddress}");
            content.Append($"|8{order.Phone}");
            content.Append($"|8{order.GetUserCall()}");
            if (!string.IsNullOrEmpty(business.AppQrCode))
            {
                content.Append($"|2{business.AppQrCode}");
            }
            content.Append($"|6********#{order.Identifier}完********");


            var postData = JsonConvert.SerializeObject(new
            {
                number = device.Code,
                app_key,
                token = device.Remark,
                msg = content.ToString()
            });

            using (var client = new HttpClient())
            {
                var body = new StringContent(postData);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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
            str += $"|8简单猫提示您：{content}";
            str += "********************************<BR>";

            var postData = JsonConvert.SerializeObject(new
            {
                number = device.Code,
                app_key,
                token = device.Remark,
                msg = str
            });

            using (var client = new HttpClient())
            {
                var body = new StringContent(postData);
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await client.PostAsync(url, body);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
