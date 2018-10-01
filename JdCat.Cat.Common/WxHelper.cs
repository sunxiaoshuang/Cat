using JdCat.Cat.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Common
{
    public static class WxHelper
    {
        /// <summary>
        /// 记录小程序访问Token，如果换成分布式部署，需要存在Redis里面
        /// </summary>
        private static readonly Dictionary<string, WxToken> TokenDic = new Dictionary<string, WxToken>();
        /// <summary>
        /// 获取小程序Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static async Task<string> GetTokenAsync(string appId, string secret)
        {
            // 已经存在Token时，取现有的Token
            if (TokenDic.ContainsKey(appId))
            {
                var token = TokenDic[appId];
                var second = (DateTime.Now - token.GetTime.Value).TotalSeconds;
                // 如果Token没有过期，则直接返回
                if(second < token.expires_in)
                {
                    return token.access_token;
                }
                else
                {
                    // 过期后从缓存中删除
                    TokenDic.Remove(appId);
                }
            }
            // 获取小程序访问Token
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={secret}";
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                var token = JsonConvert.DeserializeObject<WxToken>(await result.Content.ReadAsStringAsync());
                if(token.errcode == null)
                {
                    token.GetTime = DateTime.Now;
                    TokenDic.Add(appId, token);
                    return token.access_token;
                }
                else
                {
                    // 获取Token失败时，重新调用访问方法
                    return await GetTokenAsync(appId, secret);
                }
            }
        }
        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<string> SendTemplateMessage(WxTemplateMessage msg)
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

    }
}
