using JdCat.Cat.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JdCat.Cat.Common
{
    public static class WxHelper1
    {
        public static string Msg_Refund;                        // 退款通知模版消息id
        public static string WeChatAppId;                       // 简单猫科技公众号AppId
        public static string WeChatSecret;                      // 简单猫科技公众号Secret
        public static string MapApiKey;                         // 腾讯地图开发者key
        public static string MapApiSecret;                      // 腾讯地图WebService接口Secret

        /// <summary>
        /// 微信卡券颜色对应表
        /// </summary>
        public static Dictionary<string, string> WxColors = new Dictionary<string, string> {    
            { "Color010", "#63b359" },
            { "Color020", "#2c9f67" },
            { "Color030", "#509fc9" },
            { "Color040", "#5885cf" },
            { "Color050", "#9062c0" },
            { "Color060", "#d09a45" },
            { "Color070", "#e4b138" },
            { "Color080", "#ee903c" },
            { "Color090", "#dd6549" },
            { "Color100", "#cc463d" }
        };
        public static void Init(AppData config)
        {
            //Msg_Refund = config.Msg_Refund;
            //WeChatAppId = config.WeChatAppId;
            //WeChatSecret = config.WeChatSecret;
            MapApiKey = config.MapApiKey;
            MapApiSecret = config.MapApiSecret;
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static async Task<string> RequestAsync(string url, HttpContent content = null, string method = "post")
        {
            method = method.ToLower();
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                switch (method)
                {
                    case "get":
                        result = await client.GetAsync(url);
                        break;
                    case "post":
                        result = await client.PostAsync(url, content);
                        break;
                    default:
                        throw new Exception($"不存在方法{method}");
                }
            }
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 开放平台授权ticket，暂时存在这里
        /// </summary>
        public static string OpenTicket { get; set; }
        /// <summary>
        /// 记录小程序访问Token，如果换成分布式部署，需要存在Redis里面
        /// </summary>
        private static readonly Dictionary<string, WxToken> TokenDic = new Dictionary<string, WxToken>();
        /// <summary>
        /// 获取Token
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

        /// <summary>
        /// 获取公众号自定义菜单
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetAppMenuAsync()
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={token}";
            return await RequestAsync(url, method: "get");
        }

        /// <summary>
        /// 创建公众号自定义菜单
        /// </summary>
        /// <returns></returns>
        public static async Task<string> CreateAppMenuAsync(object menus)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={token}";
            var content = new InputData(null);
            content.SetValue("button", menus);
            var body = new StringContent(content.ToJson());
            return await RequestAsync(url, body);
        }

        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <returns></returns>
        public static async Task<string> DeleteAppMenuAsync()
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={token}";
            return await RequestAsync(url, method: "get");
        }

        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<string> GetOpenIdAsync(string code)
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={WeChatAppId}&secret={WeChatSecret}&code={code}&grant_type=authorization_code";
            using (var hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static async Task<string> GetUserInfoAsync(string openId)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={token}&openid={openId}&lang=zh_CN";
            return await RequestAsync(url, null, method: "get");
        }

        #region 第三方开发平台业务方法

        private static WxToken _openToken;
        /// <summary>
        /// 获取第三方平台component_access_token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetOpenTokenAsync(AppData appData)
        {
            if (_openToken != null)
            {
                var second = (DateTime.Now - _openToken.GetTime.Value).TotalSeconds;
                // 如果Token没有过期，则直接返回
                if (second < _openToken.expires_in - 360)
                {
                    return _openToken.access_token;
                }
            }

            var url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";
            var content = new
            {
                component_appid = appData.OpenAppId,
                component_appsecret = appData.OpenSecret,
                component_verify_ticket = OpenTicket
            };
            using (var client = new HttpClient())
            using (var body = new StringContent(JsonConvert.SerializeObject(content)))
            {
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(result);
                var jToken = jObj["component_access_token"];
                if (jToken == null)
                {
                    return null;
                }
                var token = jObj["component_access_token"].Value<string>();
                var expires_in = jObj["expires_in"].Value<int>();
                _openToken = new WxToken { access_token = token, GetTime = DateTime.Now, expires_in = expires_in };
                return token;
            }
        }
        /// <summary>
        /// 获取预授权码pre_auth_code
        /// </summary>
        /// <param name="component_appid">第三方平台AppId</param>
        /// <returns></returns>
        public static async Task<string> GetOpenPreAuthCodeAsync(AppData appData)
        {
            var token = await GetOpenTokenAsync(appData);
            if (string.IsNullOrEmpty(token)) return null;
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token={token}";
            var content = new { component_appid = appData.OpenAppId };
            using (var client = new HttpClient())
            using (var body = new StringContent(JsonConvert.SerializeObject(content)))
            {
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(result);
                var jCode = jObj["pre_auth_code"];
                if (jCode == null)
                {
                    return null;
                }
                return jObj["pre_auth_code"].Value<string>();
            }
        }
        /// <summary>
        /// 第三方平台授权访问token
        /// </summary>
        private static Dictionary<string, WxToken> _authAccessTokenDic = new Dictionary<string, WxToken>();
        /// <summary>
        /// 根据授权码获取接口凭据和授权信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<WxAuthInfo> GetAuthTokenAsync(AppData appData, string code)
        {
            var token = await GetOpenTokenAsync(appData);
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token={token}";
            var content = new
            {
                component_appid = appData.OpenAppId,
                authorization_code = code
            };
            using (var client = new HttpClient())
            using (var body = new StringContent(JsonConvert.SerializeObject(content)))
            {
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                var entity = JsonConvert.DeserializeObject<WxAuthInfo>(result);
                var wxToken = new WxToken
                {
                    access_token = entity.authorization_info.authorizer_access_token,
                    expires_in = entity.authorization_info.expires_in,
                    GetTime = DateTime.Now
                };
                SetAuthorizerAccessToken(entity.authorization_info.authorizer_appid, wxToken);
                return entity;
            }
        }
        /// <summary>
        /// 刷新接口凭据
        /// </summary>
        /// <returns></returns>
        public static async Task<string> RefreshTokenAsync(AppData appData, string appid, string refreshToken)
        {
            var token = await GetOpenTokenAsync(appData);
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token={token}";
            var content = new
            {
                component_appid = appData.OpenAppId,
                authorizer_appid = appid,
                authorizer_refresh_token = refreshToken
            };
            var sendData = JsonConvert.SerializeObject(content);
            var result = await PostAsync(url, sendData);
            var jObj = JObject.Parse(result);
            var wxToken = new WxToken
            {
                access_token = jObj["authorizer_access_token"].Value<string>(),
                expires_in = jObj["expires_in"].Value<int>(),
                GetTime = DateTime.Now
            };
            SetAuthorizerAccessToken(appid, wxToken);
            return wxToken.access_token;
        }
        /// <summary>
        /// 设置接口凭据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        private static void SetAuthorizerAccessToken(string appId, WxToken token)
        {
            if (_authAccessTokenDic.ContainsKey(appId))
            {
                _authAccessTokenDic[appId] = token;
            }
            else
            {
                _authAccessTokenDic.Add(appId, token);
            }
        }
        /// <summary>
        /// 获取接口凭据
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetAuthorizerAccessTokenAsync(AppData appData, string appId, string refreshToken)
        {
            WxToken token;
            if (_authAccessTokenDic.ContainsKey(appId))
            {
                token = _authAccessTokenDic[appId];
                if (!token.IsExpires())
                {
                    return token.access_token;
                }
            }
            return await RefreshTokenAsync(appData, appId, refreshToken);
        }
        /// <summary>
        /// 获取授权方帐号信息
        /// </summary>
        /// <param name="component_appid"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static async Task<string> GetAuthorizerInfoAsync(AppData appData, string appId)
        {
            var token = await GetOpenTokenAsync(appData);
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token={token}";
            var content = new
            {
                component_appid = appData.OpenAppId,
                authorizer_appid = appId
            };
            var body = JsonConvert.SerializeObject(content);
            var result = await PostAsync(url, body);
            return result;
        }
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private static async Task<string> PostAsync(string url, string content)
        {
            using (var client = new HttpClient())
            using (var body = new StringContent(content))
            {
                var res = await client.PostAsync(url, body);
                var result = await res.Content.ReadAsStringAsync();
                return result;
            }
        }

        #endregion

        #region 腾讯地图接口

        private static string mapApiUrl = "https://apis.map.qq.com";
        private static Dictionary<string, string> mapApiDic = new Dictionary<string, string>();
        /// <summary>
        /// 根据经纬度获取周边地址信息
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<string> GetAddressAsync(string location)
        {
            if (mapApiDic.ContainsKey(location))
            {
                return mapApiDic[location];
            }
            var path = "/ws/geocoder/v1";
            var signContent = $"{path}?key={MapApiKey}&location={location}{MapApiSecret}";
            var sign = UtilHelper.MD5Encrypt(signContent);
            var url = $"{mapApiUrl}{path}?key={MapApiKey}&location={location}&sig={sign}";
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(url);
                res.EnsureSuccessStatusCode();
                var result = await res.Content.ReadAsStringAsync();
                mapApiDic.Add(location, result);
                return result;
            }
        }

        #endregion

        #region 卡券

        /// <summary>
        /// 创建卡券
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static async Task<string> CreateCardAsync(JObject json)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/create?access_token={token}", new StringContent(JsonConvert.SerializeObject(json)));
        }

        /// <summary>
        /// 根据CardId获取卡券详情
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public static async Task<string> GetCardAsync(string cardId)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/get?access_token={token}", new StringContent(JsonConvert.SerializeObject(new { card_id = cardId })));
        }

        /// <summary>
        /// 设置公众号白名单
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> SetWhiteListAsync(object obj)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/testwhitelist/set?access_token={token}", new StringContent(JsonConvert.SerializeObject(obj)));
        }

        /// <summary>
        /// 获取卡券二维码信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> GetCardQrcodeAsync(object obj)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/qrcode/create?access_token={token}", new StringContent(JsonConvert.SerializeObject(obj)));
        }

        /// <summary>
        /// 更新会员卡信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> UpdateCardAsync(object obj)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/update?access_token={token}", new StringContent(JsonConvert.SerializeObject(obj)));
        }

        /// <summary>
        /// 设置会员卡表单内容
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> SetMemberCardActiveOptionAsync(object obj)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/membercard/activateuserform/set?access_token={token}", new StringContent(JsonConvert.SerializeObject(obj)));
        }

        /// <summary>
        /// 拉取会员信息（积分查询）
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<string> GetMemberInfoAsync(string cardId, string code)
        {
            var token = await GetTokenAsync(WeChatAppId, WeChatSecret);
            return await RequestAsync($"https://api.weixin.qq.com/card/membercard/userinfo/get?access_token={token}", new StringContent(JsonConvert.SerializeObject(new { card_id = cardId, code })));
        }

        #endregion
    }
}
