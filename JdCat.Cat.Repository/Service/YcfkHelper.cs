using JdCat.Cat.Common;
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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Repository.Service
{
    /// <summary>
    /// 一城飞客帮助类
    /// </summary>
    public class YcfkHelper
    {
        private static YcfkHelper helper;
        static YcfkHelper()
        {
            helper = new YcfkHelper();
        }
        protected YcfkHelper()
        {

        }
        public static YcfkHelper GetHelper() => helper;

        public string Domain { get; set; }
        public string PartnerKey { get; set; }
        public string Secret { get; set; }

        public YcfkHelper Init(AppData appData)
        {
            PartnerKey = appData.YcfkPartnerKey;
            Secret = appData.YcfkSecret;
            Domain = appData.YcfkDomain + "ClientOrder.ashx";
            return this;
        }

        /// <summary>
        /// 发送订单
        /// </summary>
        /// <returns></returns>
        public async Task<string> Send(YcfkOrder order)
        {
            var action = "SendOrderData";
            var content = JsonConvert.SerializeObject(order);
            return await Request(action, content);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        public async Task<string> Cancel(string OrderCode, string Reason)
        {
            var action = "CloseOrder";
            var content = JsonConvert.SerializeObject(new { OrderId = OrderCode, Reason });
            return await Request(action, content);
        }

        /// <summary>
        /// 创建签名
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public string CreateSignature(string content, string ts)
        {
            var signature = $"{PartnerKey}{content}{Secret}{ts}";
            signature = UtilHelper.SHA1(signature).ToLower();
            return signature;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<string> Request(string action, string content)
        {
            //content = content.Replace("\"", "'");
            
            var query = CreateRequestString(content, action);
            using (var client = new HttpClient())
            using (var body = new StringContent(query))
            {
                body.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                body.Headers.ContentEncoding.Add("utf-8");
                var res = await client.PostAsync(Domain, body);
                var result = await res.Content.ReadAsStringAsync();
                return result;
            }
        }

        /// <summary>
        /// 创建请求字符串
        /// </summary>
        /// <param name="content">请求内容</param>
        /// <param name="action">接口</param>
        /// <returns></returns>
        private string CreateRequestString(string content, string action)
        {
            content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            var ts = DateTime.Now.ToInt();
            var signature = CreateSignature(content, ts.ToString());
            var query = $"action={action}&sign={signature}&ts={ts}&key={PartnerKey}&content={content.ToUrlEncoding()}";
            return query;
        }

    }
}
