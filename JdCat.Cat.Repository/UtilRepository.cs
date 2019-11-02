using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JdCat.Cat.Repository
{
    public class UtilRepository : BaseRepository<Business>, IUtilRepository
    {
        private StackExchange.Redis.IDatabase _database;
        public UtilRepository(CatDbContext context, StackExchange.Redis.IConnectionMultiplexer connection) : base(context)
        {
            _database = connection.GetDatabase();
        }

        public async Task<object> WxMsgHandlerAsync(WxEvent e)
        {
            if (e.MsgType == "event" && e.Event == "SCAN")
            {
                await AddListenerAsync(e);
                return "ok";
            }
            else if (e.MsgType == "event" && e.Event == "submit_membercard_user_info")
            {
                await MemberActiveAsync(e);
                return "ok";
            }
            return null;
        }

        /// <summary>
        /// 记录小程序访问Token，如果换成分布式部署，需要存在Redis里面
        /// </summary>
        private static Dictionary<string, WxToken> TokenDic { get; } = new Dictionary<string, WxToken>();
        public async Task<string> GetTokenAsync(string appId, string secret)
        {
            // 已经存在Token时，取现有的Token
            if (TokenDic.ContainsKey(appId))
            {
                var token = TokenDic[appId];
                var second = (DateTime.Now - token.GetTime.Value).TotalSeconds;
                // 如果Token没有过期，则直接返回
                if (second < token.expires_in - 1200)
                {
                    return token.access_token;
                }
            }
            // 重新设置Token
            return await SetTokenAsync(appId, secret);
        }

        /// <summary>
        /// 开放平台授权ticket，暂时存在这里（开放平台每10分钟推送一次）
        /// </summary>
        private static string OpenTicket { get; set; }
        public async Task<string> GetOpenTicketAsync()
        {
            return OpenTicket;
        }
        public void SetOpenTicketAsync(string ticket)
        {
            OpenTicket = ticket;
        }

        /// <summary>
        /// 开放平台访问Token，暂时存在这里，换成分布式部署后需要存放在Redis里
        /// </summary>
        private static WxToken OpenToken { get; set; }
        public async Task<string> GetOpenTokenAsync()
        {
            if (OpenToken != null)
            {
                var second = (DateTime.Now - OpenToken.GetTime.Value).TotalSeconds;
                // 如果Token没有过期，则直接返回
                if (second < OpenToken.expires_in - 360)
                {
                    return OpenToken.access_token;
                }
            }

            return await SetOpenTokenAsync();
        }

        /// <summary>
        /// 开放平台授权访问令牌，如果换成分布式部署，需要存在Redis里面
        /// </summary>
        private static Dictionary<string, WxToken> AuthAccessTokenDic { get; } = new Dictionary<string, WxToken>();
        public async Task<string> GetAuthorizerAccessTokenAsync(string appId)
        {
            if (AuthAccessTokenDic.ContainsKey(appId))
            {
                var auth = AuthAccessTokenDic[appId];
                if (!auth.IsExpires())
                {
                    return auth.access_token;
                }
            }
            return await RefreshTokenAsync(appId);
        }

        public async Task SaveAuthorizerInfoAsync(OpenAuthInfo info)
        {
            var entity = Context.OpenAuthInfos.FirstOrDefault(a => a.BusinessId == info.BusinessId);
            if (entity == null)
            {
                await Context.AddAsync(info);
            }
            else
            {
                // 修改之前的授权
                entity.AppId = info.AppId;
                entity.RefreshToken = info.RefreshToken;
                entity.ModifyTime = DateTime.Now;
            }
            await Context.SaveChangesAsync();
        }
        public async Task<OpenAuthInfo> GetAuthInfoByBusinessIdAsync(int businessId)
        {
            return await Context.OpenAuthInfos.FirstOrDefaultAsync(a => a.BusinessId == businessId);
        }

        #region 消息

        public async Task SendRefundMsgAsync(Order order)
        {
            var business = await GetAsync<Business>(order.BusinessId.Value);
            if (string.IsNullOrEmpty(business.RefundTemplateId)) return;
            List<WxListenUser> users = await Context.WxListenUsers.Where(a => a.BusinessId == business.ID).ToListAsync();
            if (users == null || users.Count == 0) return;
            var msg = new WxEventMessage
            {
                template_id = business.RefundTemplateId,
                data = new
                {
                    first = new { value = $"退款订单编号-{order.OrderCode}" },
                    keyword1 = new { value = "￥" + order.Price.Value, color = "#ff0000" },
                    keyword2 = new { value = order.RefundReason, color = "#ff0000" },
                    keyword3 = new { value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    keyword4 = new { value = "线上" },
                    keyword5 = new { value = $"{order.ReceiverName} - {order.Phone}" },
                    remark = new { value = "请尽快进入简单猫后台，订单管理 -> 实时订单 页处理。" }
                }
            };
            var token = await GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            foreach (var item in users)
            {
                msg.touser = item.openid;
                var result = await WxHelper.SendEventMessageAsync(msg, token);
                var ret = JsonConvert.DeserializeObject<WxMessageReturn>(result);
                if (ret.errcode != 0)
                {
                    Log.Info($"退款模版通知消息失败：[消息_{JsonConvert.SerializeObject(msg)}，结果_{result}]");
                }
            }
        }

        public async Task SendPaySuccessMsgAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.PrepayId)) return;
            var business = await GetAsync<Business>(order.BusinessId.Value);
            if (string.IsNullOrEmpty(business.TemplateNotifyId)) return;
            var openId = order.OpenId;
            if (string.IsNullOrEmpty(openId))
            {
                openId = (await GetAsync<User>(order.UserId.Value)).OpenId;
            }

            var msg = new WxTemplateMessage
            {
                emphasis_keyword = "keyword2.DATA",
                template_id = business.TemplateNotifyId,
                touser = openId,
                form_id = order.PrepayId,
                page = "pages/order/orderInfo/orderInfo?id=" + order.ID
            };
            var token = await GetTokenAsync(business.AppId, business.Secret);
            msg.access_token = token;
            msg.data = new
            {
                keyword1 = new { value = order.OrderCode },
                keyword2 = new { value = order.Price + "元" },
                keyword3 = new { value = order.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss") },
                keyword4 = new { value = order.ReceiverAddress },
                keyword5 = new { value = order.GetUserCall() },
                keyword6 = new { value = order.Phone }
            };
            var result = await WxHelper.SendTemplateMessageAsync(msg);
            var ret = JsonConvert.DeserializeObject<WxMessageReturn>(result);
            if (ret.errcode != 0)
            {
                Log.Info($"小程序模版通知消息失败：[消息_{JsonConvert.SerializeObject(msg)}，结果_{result}]");
            }
        }

        public async Task SendNewOrderMsgAsync(Order order)
        {
            var business = await GetAsync<Business>(order.BusinessId.Value);
            if (string.IsNullOrEmpty(business.NewOrderTemplateId)) return;
            var msg = new WxEventMessage
            {
                template_id = business.NewOrderTemplateId
            };
            var productName = string.Empty;
            foreach (var item in order.Products)
            {
                productName += item.Name + " *" + (double)item.Quantity + "、";
            }
            productName = productName.Remove(productName.Length - 1);
            var keyword5 = (order.Status == OrderStatus.Distribution || order.Status == OrderStatus.DistributorReceipt) ? "待配送" : "已付款";
            msg.data = new
            {
                first = new { value = $"流水号：   #{order.Identifier}      ￥{order.Price}" },
                keyword1 = new { value = productName },
                keyword2 = new { value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                keyword3 = new { value = order.ReceiverAddress },
                keyword4 = new { value = "   " + order.GetUserCall() + "    " + order.Phone },
                keyword5 = new { value = keyword5, color = "#ff0000" },
                remark = new { value = order.Remark }
            };
            var users = await Context.WxListenUsers.Where(a => a.BusinessId == order.BusinessId.Value).ToListAsync();
            if (users == null || users.Count == 0) return;
            var token = await GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            foreach (var item in users)
            {
                msg.touser = item.openid;
                var result = await WxHelper.SendEventMessageAsync(msg, token);

                var ret = JsonConvert.DeserializeObject<WxMessageReturn>(result);
                if (ret.errcode != 0)
                {
                    Log.Info($"公众号模版通知消息失败：[消息_{JsonConvert.SerializeObject(msg)}，结果_{result}]");
                    await WechatHandlerErrorAsync(ret, msg);
                }
            }
        }

        #endregion

        public async Task<long> GetNumberAsync()
        {
            return await _database.StringIncrementAsync("Jiandanmao:Util:1");
            //return 1;
        }


        public async Task<string> GetNextCodeForReturnCouponAsync()
        {
            var num = await _database.StringIncrementAsync("Jiandanmao:Util:ReturnCoupon:Code");
            var code = num.ToString().PadLeft(6, '0');
            var year = DateTime.Now.Year;
            var rand = UtilHelper.RandNum(4);
            return $"{year}{code}{rand}";
        }
        




        #region 私有方法

        /// <summary>
        /// 添加通知人
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task AddListenerAsync(WxEvent e)
        {
            if (!int.TryParse(e.EventKey, out int businessId))
            {
                return;
            }
            var count = Context.WxListenUsers.Count(a => a.BusinessId == businessId);
            if (count >= 4) return;     // 添加人数不能大于4
            // 已存在的不再添加
            if ((await Context.WxListenUsers.CountAsync(a => a.BusinessId == businessId && a.openid == e.FromUserName)) > 0) return;
            var business = await GetAsync<Business>(businessId);
            var token = await GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            var result = await WxHelper.GetUserInfoAsync(e.FromUserName, token);
            var user = JsonConvert.DeserializeObject<WxListenUser>(result);
            user.BusinessId = businessId;
            await Context.AddAsync(user);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 会员卡激活
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task MemberActiveAsync(WxEvent e)
        {
            var card = await Context.WxCards.FirstOrDefaultAsync(a => a.CardId == e.CardId);
            var business = await GetAsync<Business>(card.BusinessId);
            var token = await GetTokenAsync(business.WeChatAppId, business.WeChatSecret);
            var res = await WxHelper.GetMemberInfoAsync(token, e.CardId, e.UserCardCode);
            var json = JObject.Parse(res);
            if (json["errcode"].ToString() != "0")
            {
                // 获取会员信息失败，重新设置一遍Token，再试一次
                await SetTokenAsync(business.WeChatAppId, business.WeChatSecret);
                res = await WxHelper.GetMemberInfoAsync(token, e.CardId, e.UserCardCode);
                json = JObject.Parse(res);
                if (json["errcode"].ToString() != "0")
                {
                    Log.Error($"获取会员[{e.UserCardCode}]信息失败：{json["errmsg"]}");
                    return;
                }
            }
            var entity = new WxMember
            {
                Code = e.UserCardCode,
                NickName = json["nickname"].ToString(),
                CardId = e.CardId,
                WxCardId = card.ID,
                OpenId = e.FromUserName,
                BusinessId = card.BusinessId
            };
            if (json["user_info"]["common_field_list"] != null)
            {
                var arr = json["user_info"]["common_field_list"].ToArray();
                entity.Name = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_NAME")?["value"].ToString();
                entity.Phone = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_MOBILE")?["value"].ToString();
                var gender = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_SEX")?["value"].ToString();
                if (gender == "MALE") entity.Gender = UserGender.Male;
                else if (gender == "FEMALE") entity.Gender = UserGender.Female;
                var birthday = arr.FirstOrDefault(a => a["name"].ToString() == "USER_FORM_INFO_FLAG_BIRTHDAY")?["value"].ToString();
                if (birthday != null) entity.Birthday = Convert.ToDateTime(birthday);
            }
            var userStr = await WxHelper.GetUserInfoAsync(e.FromUserName, token);
            var user = JsonConvert.DeserializeObject<WxListenUser>(userStr);
            entity.Logo = user.headimgurl;
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 设置appId相关的Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private async Task<string> SetTokenAsync(string appId, string secret)
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
        /// 设置开放平台访问Token
        /// </summary>
        /// <returns></returns>
        private async Task<string> SetOpenTokenAsync()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";
            var ticket = await GetOpenTicketAsync();
            if (string.IsNullOrEmpty(ticket)) throw new ArgumentNullException("component_verify_ticket", "开放平台ticket不能为空，该值每10分钟推送一次");
            var content = new
            {
                component_appid = WxHelper.OpenAppId,
                component_appsecret = WxHelper.OpenSecret,
                component_verify_ticket = await GetOpenTicketAsync()
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
                OpenToken = new WxToken { access_token = token, GetTime = DateTime.Now, expires_in = expires_in };
                return token;
            }
        }

        /// <summary>
        /// 刷新开放平台授权访问令牌
        /// </summary>
        /// <param name="token"></param>
        /// <param name="appId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private async Task<string> RefreshTokenAsync(string appId)
        {
            var token = await GetOpenTokenAsync();
            var authInfo = await Context.OpenAuthInfos.FirstOrDefaultAsync(a => a.AppId == appId);
            if (authInfo == null) throw new Exception($"公众号未授权");
            var url = $"https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token={token}";
            var content = new
            {
                component_appid = WxHelper.OpenAppId,
                authorizer_appid = appId,
                authorizer_refresh_token = authInfo.RefreshToken
            };
            var result = await UtilHelper.RequestAsync(url, content);
            var jObj = JObject.Parse(result);
            var wxToken = new WxToken
            {
                access_token = jObj["authorizer_access_token"].Value<string>(),
                expires_in = jObj["expires_in"].Value<int>(),
                GetTime = DateTime.Now
            };
            if (AuthAccessTokenDic.ContainsKey(appId))
            {
                AuthAccessTokenDic[appId] = wxToken;
            }
            else
            {
                AuthAccessTokenDic.Add(appId, wxToken);
            }
            return wxToken.access_token;
        }

        /// <summary>
        /// 公众号消息发送错误处理
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task WechatHandlerErrorAsync(WxMessageReturn ret, WxEventMessage msg)
        {
            if(ret.errcode == 43004)
            {
                // 用户已取消关注，则删除该用户
                var openId = msg.touser;
                var users = await Context.WxListenUsers.Where(a => a.openid == openId).ToListAsync();
                if(users.Count > 0)
                {
                    Context.RemoveRange(users);
                    await Context.SaveChangesAsync();
                }
            }
        }

        #endregion

    }
}
