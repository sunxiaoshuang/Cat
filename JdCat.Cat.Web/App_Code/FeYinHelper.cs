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
        public async Task<FeyinModel> Print(string device_no, Order order)
        {
            var url = $"https://api.open.feyin.net/msg";
            var content = new StringBuilder();
            content.Append("<left><Font# Bold=1 Width=2 Height=2>商家小票</Font#>\n");
            content.Append("--------------------------------\n");
            content.Append("<center><Font# Bold=1 Width=2 Height=2>简单猫外卖</Font#>\n");
            content.Append("\n\n");
            content.Append($"<left>下单时间：{order.CreateTime:yyyy-MM-dd HH:mm:ss}\n");
            content.Append("************购买商品************\n");
            //content.Append($"******{DateTime.Now:yyyy-MM-dd HH:mm:ss}*******\n");
            if (order.Products == null || order.Products.Count == 0)
            {
                content.Append("<Font# Bold=1 Width=1 Height=1>无任何商品</Font#>\n");
            }
            else
            {
                foreach (var product in order.Products)
                {
                    var name = product.Name.Length > 8 ? product.Name.Substring(0, 7) : product.Name;
                    var buffer = Encoding.Default.GetBytes(name);
                    var len = 24 - buffer.Count();
                    if (len > 0)
                    {
                        for (int i = 0; i < len / 2; i++)
                        {
                            name += " ";
                        }
                    }
                    name += "* " + product.Quantity;
                    name += "    " + product.Price.Value.ToString("f2");
                    content.Append($"<Font# Bold=1 Width=1 Height=1>{name}</Font#>\n");
                }
                //content.Append($"*************其他*************");
                content.Append("--------------------------------\n");
                content.Append($"<right>总价：{order.Price.Value.ToString("f2")}\n");
                content.Append("--------------------------------\n");
                content.Append($"<left><Font# Bold=1 Width=2 Height=2>{order.ReceiverAddress}</Font#>\n");
                content.Append($"<Font# Bold=1 Width=2 Height=2>{order.Phone}</Font#>\n");
                content.Append($"{order.ReceiverName}\n");
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
    }
}
