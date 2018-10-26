using JdCat.Cat.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JdCat.Cat.Common
{
    public static class WxHelper
    {
        public const string WeChatAppId = "wx37df4bb420888824";                         // 简单猫科技公众号AppId
        public const string WeChatSecret = "8db34ed73016a5f22878295ed409cc52";          // 简单猫科技公众号Secret
        /// <summary>
        /// 记录小程序访问Token，如果换成分布式部署，需要存在Redis里面
        /// </summary>
        private static readonly Dictionary<string, WxToken> TokenDic = new Dictionary<string, WxToken>();
        /// <summary>
        /// 获取小程序Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="是否重置Token"></param>
        /// <returns></returns>
        public static async Task<string> GetTokenAsync(string appId, string secret)
        {
            // 已经存在Token时，取现有的Token
            if (TokenDic.ContainsKey(appId))
            {
                var token = TokenDic[appId];
                var second = (DateTime.Now - token.GetTime.Value).TotalSeconds;
                // 如果Token没有过期，则直接返回
                if (second < token.expires_in - 360)
                {
                    return token.access_token;
                }
            }
            // 重新设置Token
            return await SetTokenAsync(appId, secret);
        }
        /// <summary>
        /// 根据id与密钥，设置Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static async Task<string> SetTokenAsync(string appId, string secret)
        {
            if (TokenDic.ContainsKey(appId))
            {
                TokenDic.Remove(appId);
            }
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={secret}";
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                var token = JsonConvert.DeserializeObject<WxToken>(await result.Content.ReadAsStringAsync());
                if (token.errcode == null)
                {
                    token.GetTime = DateTime.Now;
                    TokenDic.Add(appId, token);
                    return token.access_token;
                }
            }
            return null;
        }
        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<string> SendTemplateMessageAsync(WxTemplateMessage msg)
        {
            var body = JsonConvert.SerializeObject(msg);
            var url = $"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={msg.access_token}";
            using (var client = new HttpClient())
            {
                var post = new StringContent(body);
                post.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await client.PostAsync(url, post);
                var content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }
        /// <summary>
        /// 根据商户id与Token获取公众号永久二维码
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<WxTicket> GetTicketAsync(int businessId, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={token}";
            var body = new
            {
                action_name = "QR_LIMIT_STR_SCENE",
                action_info = new { scene = new { scene_str = businessId.ToString() } }
            };
            var content = new StringContent(JsonConvert.SerializeObject(body));
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(url, content);
                var data = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WxTicket>(data);
            }
        }
        /// <summary>
        /// 发送订单通知
        /// </summary>
        public static async Task<string> SendEventMessageAsync(WxEventMessage msg)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";

            using (var client = new HttpClient())
            {
                var p = new StringContent(JsonConvert.SerializeObject(msg));
                p.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await client.PostAsync(url, p);
                var content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }

    }
}
