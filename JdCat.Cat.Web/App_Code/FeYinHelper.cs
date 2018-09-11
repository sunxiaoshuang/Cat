using JdCat.Cat.Model.Data;
using JdCat.Cat.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Web.App_Code
{
    public class FeYinHelper
    {
        private string _memberCode;
        /// <summary>
        /// 飞印商户编码
        /// </summary>
        public string MemberCode { get => _memberCode; set => _memberCode = value; }
        private string _apiKey;
        /// <summary>
        /// 飞印API密钥
        /// </summary>
        public string ApiKey { get => _apiKey; set => _apiKey = value; }
        private string appId;
        /// <summary>
        /// 飞印应用id
        /// </summary>
        public string AppId { get => appId; set => appId = value; }
        private string token;
        /// <summary>
        /// 服务令牌
        /// </summary>
        public string Token { get => token; set => token = value; }

        /// <summary>
        /// 获取token令牌
        /// </summary>
        public async Task GetToken()
        {
            var url = $"https://api.open.feyin.net/token?code={_memberCode}&secret={_apiKey}";
            var result = await Request(url, method: "get", isToken: false);
            Token = result.Access_Token;
        }
        /// <summary>
        /// 打印小票
        /// </summary>
        public async Task<FeyinModel> Print(string device_no, Order order, Business business = null)
        {
            var url = $"https://api.open.feyin.net/msg";
            var businessName = business == null ? "简单猫外卖" : business.Name;
            var content = new StringBuilder();
            content.Append($"<center><Font# Bold=1 Width=4 Height=4>#{order.Identifier}</Font#><Font# Bold=1 Width=2 Height=2> 简单猫</Font#>\n");
            content.Append("<left><Font# Bold=1 Width=1 Height=1>配送小票</Font#>\n");
            content.Append("--------------------------------\n");
            content.Append("<center><Font# Bold=1 Width=2 Height=2>" + businessName + "</Font#>\n");
            content.Append("\n\n");
            content.Append($"<left>下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}\n");
            content.Append($"<left>订单编号：{order.OrderCode}\n");
            content.Append("************购买商品************\n");
            //content.Append($"******{DateTime.Now:yyyy-MM-dd HH:mm:ss}*******\n");
            if (order.Products == null || order.Products.Count == 0)
            {
                content.Append("<Font# Bold=1 Width=1 Height=1>无任何商品</Font#>\n");
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
                        zhQuantity = CalcZhQuantity(name);
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
                    content.Append($"<Font# Bold=1 Width=1 Height=1>{name + quantity + price}</Font#>\n");
                    if(!string.IsNullOrEmpty(cutName))
                    {
                        content.Append($"<Font# Bold=1 Width=1 Height=1>{cutName}</Font#>\n");
                    }
                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        content.Append($"<Font# Bold=0 Width=1 Height=1>（{product.Description}）</Font#>\n");
                    }
                }
                content.Append("--------------其他--------------\n");
                content.Append($"<left>{PrintLineLeftRight("配送费", Convert.ToDouble(order.Freight.Value) + "")}\n");
                if (order.SaleCouponUser != null)
                {
                    content.Append($"{PrintLineLeftRight("[" + order.SaleCouponUser.Name + "]", "-￥" + Convert.ToDouble(order.SaleCouponUser.Value) + "")}\n");
                }
                if (order.SaleFullReduce != null)
                {
                    content.Append($"{PrintLineLeftRight("[" + order.SaleFullReduce.Name + "]", "-￥" + Convert.ToDouble(order.SaleFullReduce.ReduceMoney) + "")}\n");
                }
                content.Append("--------------------------------\n");
                if (order.SaleFullReduce != null)
                {
                    content.Append($"<right>原价：{Convert.ToDouble(order.OldPrice.Value)}元\n");
                }
                content.Append($"<right>实付：<Font# Bold=1 Width=2 Height=2>{Convert.ToDouble(order.Price)}元</Font#>\n");
                if (!string.IsNullOrEmpty(order.Remark))
                {
                    content.Append($"<left>备注：{order.Remark}\n");
                }
                content.Append("********************************\n");
                content.Append($"<left><Font# Bold=1 Width=2 Height=2>{order.ReceiverAddress}</Font#>\n");
                content.Append($"<Font# Bold=1 Width=2 Height=2>{order.Phone}</Font#>\n");
                content.Append($"<Font# Bold=1 Width=2 Height=2>{order.ReceiverName}</Font#>\n");
                content.Append($"<center>***********<Font# Bold=1 Width=2 Height=2>#{order.Identifier}</Font#>完***********\n");
            }

            return await Request(url, new
            {
                device_no,
                msg_no = Guid.NewGuid().ToString(),//order.OrderCode,
                appid = appId,
                msg_content = content.ToString()
            });
        }

        /// <summary>
        /// 添加模版
        /// </summary>
        public void AddTemplate()
        {

        }
        /// <summary>
        /// 绑定设备
        /// </summary>
        public async Task<FeyinModel> BindDevice(string device_no)
        {
            var url = $"https://api.open.feyin.net/device/{device_no}/bind";
            return await Request(url);
        }
        /// <summary>
        /// 解除设备
        /// </summary>
        public async Task<FeyinModel> UnBindDevice(string device_no)
        {
            var url = $"https://api.open.feyin.net/device/{device_no}/unbind";
            return await Request(url);
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private async Task<FeyinModel> Request(string url, object p = null, string method = "post", bool isToken = true)
        {
            var api = url;
            if (isToken)
            {
                api = url + $"?access_token={Token}";
            }
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = null;
                StringContent body = null;
                if (p != null)
                {
                    body = new StringContent(JsonConvert.SerializeObject(p));
                    body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                if (method == "get")
                {
                    result = await client.GetAsync(api);
                }
                else
                {
                    result = await client.PostAsync(api, body);
                }
                var msg = await result.Content.ReadAsStringAsync();
                var ret = JsonConvert.DeserializeObject<FeyinModel>(msg);
                // 如果返回的结果是40014【token不存在或已过期】，则重新请求服务器得到Token
                if (ret.ErrCode == 40014)
                {
                    await GetToken();
                    return await Request(url, p, method);
                }
                return ret;
            }
        }

        /// <summary>
        /// 计算出文本中的中文字符数量
        /// </summary>
        /// <returns></returns>
        private int CalcZhQuantity(string text)
        {
            // 一个中文字符占用三个字节，一个其他字符占用一个字节
            var len = text.Length;
            var byteLen = Encoding.UTF8.GetByteCount(text);
            return (byteLen - len) / 2;
        }
        /// <summary>
        /// 打印一行，左右两边对齐
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private string PrintLineLeftRight(string left, string right, int width = 32)
        {
            var zhLeft = CalcZhQuantity(left);          // 左边文本的中文字符长度
            var enLeft = left.Length - zhLeft;          // 左边文本的其他字符长度
            var zhRight = CalcZhQuantity(right);        // 右边文本的中文字符长度
            var enRight = right.Length - zhRight;       // 右边文本的其他字符长度
            var len = width - (zhLeft * 2 + enLeft + zhRight * 2 + enRight);            // 缺少的字符长度
            for (int i = 0; i < len; i++)
            {
                left += " ";
            }
            return left + right;
        }
    }
}
