using System;
using System.Collections.Generic;
using System.Linq;
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
        public UtilRepository(CatDbContext context) : base(context)
        {
        }

        public async Task<object> WxMsgHandlerAsync(WxEvent e)
        {
            if(e.MsgType == "event" && e.Event == "SCAN")
            {
                await AddListenerAsync(e);
                return "ok";
            }
            else if(e.MsgType == "event" && e.Event == "submit_membercard_user_info")
            {
                await MemberActiveAsync(e);
                return "ok";
            }
            return null;
        }


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
            var count = await Context.WxListenUsers.AsNoTracking().Where(a => a.BusinessId == businessId).CountAsync();
            if (count >= 4) return;     // 添加人数不能大于4
            // 已存在的不再添加
            if ((await Context.WxListenUsers.CountAsync(a => a.BusinessId == businessId && a.openid == e.FromUserName)) > 0) return;

            var result = await WxHelper.GetUserInfoAsync(e.FromUserName);
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
            var res = await WxHelper.GetMemberInfoAsync(e.CardId, e.UserCardCode);
            var json = JObject.Parse(res);
            if (json["errcode"].ToString() != "0")
            {
                Log.Error($"获取会员[{e.UserCardCode}]信息失败：{json["errmsg"]}");
                return;
            }
            var card = await Context.WxCards.FirstOrDefaultAsync(a => a.CardId == e.CardId);
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
            var userStr = await WxHelper.GetUserInfoAsync(e.FromUserName);
            var user = JsonConvert.DeserializeObject<WxListenUser>(userStr);
            entity.Logo = user.headimgurl;
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

    }
}
