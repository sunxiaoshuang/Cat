using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace JdCat.Cat.Web.Controllers
{
    public class CardController : BaseController<ICardRepository, WxCard>
    {
        public CardController(AppData appData, ICardRepository service) : base(appData, service)
        {
        }

        #region 会员卡

        public IActionResult MemberCard()
        {
            ViewBag.mpUrl = AppData.MpUrl;
            return View();
        }

        /// <summary>
        /// 创建会员卡
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMemberCard([FromServices]IUtilRepository util)
        {
            var ret = new JsonData();
            using (StreamReader d = new StreamReader(Request.Body))
            {
                var str = await d.ReadToEndAsync();
                var json = JObject.Parse(str);
                json["card"]["member_card"]["base_info"]["logo_url"] = $"{AppData.FileUri}/File/Logo/{Business.ID}/{Business.LogoSrc}";

                LoadRule((JObject)json["card"], out List<CardChargeRule> charges, out CardBonusRule bonusSale, out CardBonusRule bonusCharge, out CardBonusRule bonusOpen);

                var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
                var msg = await WxHelper.CreateCardAsync(token, json);
                var result = JObject.Parse(msg);
                if (result["errcode"].ToString() != "0")
                {
                    ret.Msg = result["errmsg"].ToString();
                    return Json(ret);
                }
                var data = new WxCard { BusinessId = Business.ID, CardId = result["card_id"].ToString(), Category = CardCategory.MemberCard, Name = json["card"]["member_card"]["base_info"]["brand_name"].ToString(), Status = EntityStatus.Normal, Color = json["card"]["member_card"]["base_info"]["color"].ToString() };
                var card = await Service.AddAsync(data);

                charges.ForEach(a => a.WxCardId = card.ID);
                bonusSale.WxCardId = card.ID;
                bonusCharge.WxCardId = card.ID;
                bonusOpen.WxCardId = card.ID;
                await Service.SaveCardChargesAsync(charges);
                await Service.SaveCardBonusAsync(new[] { bonusSale, bonusCharge, bonusOpen });

                SetMemberCardActiveOptionAsync(card);
                ret.Success = true;
                ret.Msg = "创建成功";
                ret.Data = data.CardId;
                return Json(ret);
            }
        }

        /// <summary>
        /// 会员卡规则
        /// </summary>
        /// <param name="json"></param>
        /// <param name="charges"></param>
        /// <param name="bonusSale"></param>
        /// <param name="bonusCharge"></param>
        /// <param name="bonusOpen"></param>
        private void LoadRule(JObject card, out List<CardChargeRule> charges, out CardBonusRule bonusSale, out CardBonusRule bonusCharge, out CardBonusRule bonusOpen)
        {
            charges = JsonConvert.DeserializeObject<List<CardChargeRule>>(card["chargeList"].ToString());
            bonusSale = JsonConvert.DeserializeObject<CardBonusRule>(card["bonusSale"].ToString());
            bonusCharge = JsonConvert.DeserializeObject<CardBonusRule>(card["bonusCharge"].ToString());
            bonusOpen = JsonConvert.DeserializeObject<CardBonusRule>(card["bonusOpen"].ToString());

            card.Remove("chargeList");
            card.Remove("bonusSale");
            card.Remove("bonusCharge");
            card.Remove("bonusOpen");
        }
        /// <summary>
        /// 设置一键激活的表单
        /// </summary>
        private async void SetMemberCardActiveOptionAsync(WxCard card)
        {
            var body = new
            {
                card_id = card.CardId,
                required_form = new         // 姓名，手机号必填
                {
                    can_modify = false,
                    common_field_id_list = new[] { "USER_FORM_INFO_FLAG_NAME", "USER_FORM_INFO_FLAG_MOBILE" }
                },
                optional_form = new         // 性别，生日选填
                {
                    can_modify = false,
                    common_field_id_list = new[] { "USER_FORM_INFO_FLAG_SEX", "USER_FORM_INFO_FLAG_BIRTHDAY" }
                }
            };

            var util = HttpContext.RequestServices.GetService<IUtilRepository>();
            var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
            await WxHelper.SetMemberCardActiveOptionAsync(token, body);
        }

        /// <summary>
        /// 获取会员卡信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMemberCard()
        {
            var result = new JsonData();
            var cards = await Service.GetCardsAsync(Business.ID);
            var memberCard = cards.FirstOrDefault(a => a.Category == CardCategory.MemberCard);
            if (memberCard == null)
            {
                result.Msg = "尚未创建会员卡";
                return Json(result);
            }
            var util = HttpContext.RequestServices.GetService<IUtilRepository>();
            var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
            var card = await WxHelper.GetCardAsync(token, memberCard.CardId);
            return Content(card);
        }

        /// <summary>
        /// 获取会员储值规则、积分规则
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCardRule([FromServices]ICardRepository req)
        {
            var cards = await Service.GetCardsAsync(Business.ID);
            var memberCard = cards.FirstOrDefault(a => a.Category == CardCategory.MemberCard);
            if (memberCard == null) return Json(new { charge = new string[] { }, bonus = new string[] { } });
            var chargeRules = (await req.GetCardChargesAsync(memberCard.ID)).Select(a => new { a.Amount, a.Give });
            var bonusRules = (await req.GetCardBonusAsync(memberCard.ID)).Select(a => new { a.Amount, a.Give, a.Mode });
            return Json(new { charge = chargeRules, bonus = bonusRules });
        }

        /// <summary>
        /// 设置卡券白名单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SetWhiteList([FromServices]IUtilRepository util)
        {
            var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
            var result = await WxHelper.SetWhiteListAsync(token, new { username = new[] { "sunxsalyr", "wuliao944" } });
            return Content(result);
        }

        /// <summary>
        /// 创建会员卡二维码图片
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateMemberQrcode([FromQuery]string cardId, [FromServices]IUtilRepository util)
        {
            var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
            var result = await WxHelper.GetCardQrcodeAsync(token, new { action_name = "QR_CARD", action_info = new { card = new { card_id = cardId, outer_str = "m1" } } });
            var json = JObject.Parse(result);
            var qrcode = UtilHelper.CreateCodeEwm(json["url"].ToString());
            return File(qrcode, "image/jpeg");
        }

        public async Task<IActionResult> UpdateCard([FromServices]ICardRepository service, [FromServices]IUtilRepository util)
        {
            var ret = new JsonData();
            using (StreamReader d = new StreamReader(Request.Body))
            {
                var str = await d.ReadToEndAsync();
                var json = JObject.Parse(str);

                LoadRule(json, out List<CardChargeRule> charges, out CardBonusRule bonusSale, out CardBonusRule bonusCharge, out CardBonusRule bonusOpen);

                var token = await util.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
                var msg = await WxHelper.UpdateCardAsync(token, json);
                var result = JObject.Parse(msg);
                if (result["errcode"].ToString() != "0")
                {
                    ret.Msg = result["errmsg"].ToString();
                    return Json(ret);
                }
                var card = await service.GetCardAsync(json["card_id"].ToString());
                card.Color = json["member_card"]["base_info"]["color"].ToString();
                await service.CommitAsync();
                await Service.SaveCardChargesAsync(charges);
                await Service.SaveCardBonusAsync(new[] { bonusSale, bonusCharge, bonusOpen });
                ret.Success = true;
                ret.Msg = "修改成功";
                return Json(ret);
            }
        }


        #endregion

        /// <summary>
        /// 删除卡券
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RemoveCard([FromQuery]string cardId, [FromServices]IUtilRepository rep)
        {
            var token = await rep.GetTokenAsync(Business.WeChatAppId, Business.WeChatSecret);
            await WxHelper.RemoveCardAsync(token, cardId);
            return Ok("删除成功");
        }

    }
}
