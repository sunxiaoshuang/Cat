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
    public class WxHelper
    {
        public static string MapApiKey;                         // 腾讯地图开发者key
        public static string MapApiSecret;                      // 腾讯地图WebService接口Secret
        public static string OpenAppId;                         // 开放平台AppId
        public static string OpenSecret;                        // 开放平台Secret

        /// <summary>
        /// 微信卡券颜色对应表
        /// </summary>
        public static Dictionary<string, string> WxColors { get; } = new Dictionary<string, string>
        {
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

        /// <summary>
        /// 帮助类初始化
        /// </summary>
        /// <param name="config"></param>
        public static void Init(AppData config)
        {
            MapApiKey = config.MapApiKey;
            MapApiSecret = config.MapApiSecret;
            OpenAppId = config.OpenAppId;
            OpenSecret = config.OpenSecret;
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> RequestAsync(string url, object content = null, string method = "post")
        {
            return await UtilHelper.RequestAsync(url, content, method); ;
        }

        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<string> SendTemplateMessageAsync(WxTemplateMessage msg)
        {
            //var body = JsonConvert.SerializeObject(msg);
            var url = $"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={msg.access_token}";
            //msg.access_token = null;
            return await RequestAsync(url, msg);
            //using (var client = new HttpClient())
            //{
            //    var post = new StringContent(body);
            //    post.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    var result = await client.PostAsync(url, post);
            //    var content = await result.Content.ReadAsStringAsync();
            //    return content;
            //}
        }
        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<string> SendTemplateMessageAsync(Dictionary<string, object> dic)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={dic["access_token"]}";
            return await RequestAsync(url, dic);
        }
        /// <summary>
        /// 根据商户id与Token获取公众号永久二维码
        /// </summary>
        /// <param name="businessId">商户id</param>
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
            var result = await RequestAsync(url, body);
            return JsonConvert.DeserializeObject<WxTicket>(result);
        }
        /// <summary>
        /// 发送订单通知
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="token">公众号访问token</param>
        /// <returns></returns>
        public static async Task<string> SendEventMessageAsync(WxEventMessage msg, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
            return await RequestAsync(url, msg);
        }
        /// <summary>
        /// 发送订单通知
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="token">公众号访问token</param>
        /// <returns></returns>
        public static async Task<string> SendUniformMessageAsync(object msg, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/uniform_send?access_token={token}";
            return await RequestAsync(url, msg);
        }

        /// <summary>
        /// 获取公众号自定义菜单
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <returns></returns>
        public static async Task<string> GetAppMenuAsync(string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={token}";
            return await RequestAsync(url, method: "get");
        }

        /// <summary>
        /// 创建公众号自定义菜单
        /// </summary>
        /// <returns></returns>
        public static async Task<string> CreateAppMenuAsync(object menus, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={token}";
            var content = new InputData(null);
            content.SetValue("button", menus);
            return await RequestAsync(url, content.ToJson());
        }

        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <returns></returns>
        public static async Task<string> DeleteAppMenuAsync(string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={token}";
            return await RequestAsync(url, method: "get");
        }

        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="code">授权码</param>
        /// <param name="appId">公众号appid</param>
        /// <param name="secret">公众号密钥</param>
        /// <returns></returns>
        public static async Task<string> GetOpenIdAsync(string code, string appId, string secret)
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
            return await RequestAsync(url, method: "get");
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openId">用户openid</param>
        /// <param name="token">公众号访问token</param>
        /// <returns></returns>
        public static async Task<string> GetUserInfoAsync(string openId, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={token}&openid={openId}&lang=zh_CN";
            return await RequestAsync(url, null, method: "get");
        }

        /// <summary>
        /// 微信统一下单
        /// </summary>
        /// <param name="xml">xml格式字符串</param>
        /// <returns></returns>
        public static async Task<WxUnifieResult> UnifiedOrderAsync(string xml)
        {
            var content = await RequestAsync("https://api.mch.weixin.qq.com/pay/unifiedorder", xml);
            return UtilHelper.ReadXml<WxUnifieResult>(content);
        }

        /// <summary>
        /// 会员激活通知
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ActiveNotifyAsync(string token, JObject json)
        {
            var url = $"https://api.weixin.qq.com/card/update?access_token={token}";

            return null;
        }

        #region 第三方开发平台业务方法

        /// <summary>
        /// 获取预授权码pre_auth_code
        /// </summary>
        /// <param name="token">开放平台访问token</param>
        /// <returns></returns>
        public static async Task<string> GetOpenPreAuthCodeAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token={token}";
            var content = new { component_appid = OpenAppId };
            var result = await RequestAsync(url, content);
            var jObj = JObject.Parse(result);
            var jCode = jObj["pre_auth_code"];
            if (jCode == null)
            {
                return null;
            }
            return jObj["pre_auth_code"].Value<string>();
        }
        /// <summary>
        /// 根据授权码获取接口凭据和授权信息
        /// </summary>
        /// <param name="code">授权码</param>
        /// <param name="token">开放平台访问token</param>
        /// <returns></returns>
        public static async Task<WxAuthInfo> GetAuthTokenAsync(string code, string token)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token={token}";
            var content = new
            {
                component_appid = OpenAppId,
                authorization_code = code
            };
            var result = await RequestAsync(url, content);
            var entity = JsonConvert.DeserializeObject<WxAuthInfo>(result);
            var wxToken = new WxToken
            {
                access_token = entity.authorization_info.authorizer_access_token,
                expires_in = entity.authorization_info.expires_in,
                GetTime = DateTime.Now
            };
            return entity;
        }
        /// <summary>
        /// 获取授权方帐号信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static async Task<string> GetAuthorizerInfoAsync(string token, string appId)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token={token}";
            var content = new
            {
                component_appid = OpenAppId,
                authorizer_appid = appId
            };
            return await RequestAsync(url, content);
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
                if (!mapApiDic.ContainsKey(location))
                {
                    mapApiDic.Add(location, result);
                }
                return result;
            }
        }

        #endregion

        #region 卡券

        /// <summary>
        /// 创建卡券
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="json">卡券信息</param>
        /// <returns></returns>
        public static async Task<string> CreateCardAsync(string token, JObject json)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/create?access_token={token}", json);
        }

        /// <summary>
        /// 根据CardId获取卡券详情
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="cardId">卡券id</param>
        /// <returns></returns>
        public static async Task<string> GetCardAsync(string token, string cardId)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/get?access_token={token}", new { card_id = cardId });
        }

        /// <summary>
        /// 设置公众号白名单
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="obj">白名单列表</param>
        /// <returns></returns>
        public static async Task<string> SetWhiteListAsync(string token, object obj)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/testwhitelist/set?access_token={token}", obj);
        }

        /// <summary>
        /// 获取卡券二维码信息
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="obj">二维码信息</param>
        /// <returns></returns>
        public static async Task<string> GetCardQrcodeAsync(string token, object obj)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/qrcode/create?access_token={token}", obj);
        }

        /// <summary>
        /// 更新会员卡信息
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="obj">更新信息</param>
        /// <returns></returns>
        public static async Task<string> UpdateCardAsync(string token, object obj)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/update?access_token={token}", obj);
        }

        /// <summary>
        /// 设置会员卡表单内容
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="obj">表单信息</param>
        /// <returns></returns>
        public static async Task<string> SetMemberCardActiveOptionAsync(string token, object obj)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/membercard/activateuserform/set?access_token={token}", obj);
        }

        /// <summary>
        /// 拉取会员信息（积分查询）
        /// </summary>
        /// <param name="token">公众号访问token</param>
        /// <param name="cardId">卡券id</param>
        /// <param name="code">会员编号</param>
        /// <returns></returns>
        public static async Task<string> GetMemberInfoAsync(string token, string cardId, string code)
        {
            return await RequestAsync($"https://api.weixin.qq.com/card/membercard/userinfo/get?access_token={token}", new { card_id = cardId, code });
        }

        /// <summary>
        /// 删除卡券
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<string> RemoveCardAsync(string token, string cardId)
        {
            var url = $"https://api.weixin.qq.com/card/delete?access_token={token}";
            return await RequestAsync(url, new { card_id = cardId });
        }

        #endregion
    }
}
