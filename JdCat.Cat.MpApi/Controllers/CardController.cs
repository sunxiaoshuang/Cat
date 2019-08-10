using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.MpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSameDomain")]
    public class CardController : BaseController<ICardRepository, WxCard>
    {
        public CardController(ICardRepository service) : base(service)
        {
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpGet("member")]
        public async Task<IActionResult> GetMember([FromQuery]string cardId, [FromQuery]string openId)
        {
            var member = await Service.GetMemberAsync(cardId, openId);
            return Json(member);
        }

        /// <summary>
        /// 获取支付码
        /// </summary>
        /// <param name="id">会员id</param>
        /// <returns></returns>
        [HttpGet("paycode")]
        public async Task<IActionResult> GetPayCode([FromQuery]int id)
        {
            var payment = await Service.CreatePaymentTargetAsync(id);
            return Json(new JsonData { Success = true, Data = payment.Code });
        }

        /// <summary>
        /// 根据支付码获取条形码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("barcode")]
        public IActionResult Barcode([FromQuery]string code)
        {
            var file = UtilHelper.CreateCodeTxm(code);
            return File(file, "image/jpeg");
        }

        /// <summary>
        /// 根据支付码获取二维码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("qrcode")]
        public IActionResult Qrcode([FromQuery]string code)
        {
            var file = UtilHelper.CreateCodeEwm(code, 231, 231);
            return File(file, "image/jpeg");
        }

        /// <summary>
        /// 根据支付码获取支付结果
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("payResult")]
        public async Task<IActionResult> PayResult(string code)
        {
            return Json(await Service.GetPaymentTargetAsync(code));
        }

        /// <summary>
        /// 获取会员卡充值规则、积分规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("cardrule")]
        public async Task<IActionResult> GetCardRule([FromQuery]string id)
        {
            var card = await Service.GetCardAsync(id);
            var chargeRules = (await Service.GetCardChargesAsync(card.ID)).Select(a => new { a.Amount, a.Give });
            var bonusRule = (await Service.GetCardBonusAsync(card.ID)).FirstOrDefault(a => a.Mode == BonusMode.Charge);
            return Json(new { chargeRules, bonusRule });
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="id">会员id</param>
        /// <param name="amoumt">充值金额（单位:分）</param>
        /// <param name="give">赠送金额（单位:分）</param>
        /// <returns></returns>
        [HttpPost("pay")]
        public async Task<IActionResult> UnifiedOrder([FromQuery]int id, [FromBody]ChargeRecord record, [FromServices]AppData appData)
        {
            var member = await Service.GetAsync<WxMember>(id);
            var business = await Service.GetAsync<Business>(member.BusinessId);

            record.BusinessId = business.ID;
            record.Code = $"{DateTime.Now:yyyyMMddHHmmss}0{UtilHelper.RandNum(8)}";
            record.RelativeId = member.ID;
            await Service.AddAsync(record);

            var option = new WxUnifiePayment
            {
                appid = business.PayServerAppId,
                mch_id = business.PayServerMchId,
                sub_appid = business.WeChatAppId,
                sub_mch_id = business.MchId,
                device_info = "WEB",
                body = $"{business.Name}-会员充值",
                attach = record.ID.ToString(),
                out_trade_no = record.Code,
                total_fee = record.Amount,
                key = business.PayServerKey,
                notify_url = appData.Domain + "/api/card/paySuccess",
                spbill_create_ip = appData.HostIpAddress,
                sub_openid = record.OpenId
            };
            Log.Debug(option.notify_url);
            if (appData.RunMode == "test" || business.ID == 1)
            {
                option.total_fee = 1;
            }
            option.Generator();
            var xml = string.Empty;
            using (var stream = new MemoryStream())
            {
                UtilHelper.XmlSerializeInternal(stream, option);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var ret = await WxHelper.UnifiedOrderAsync(xml);
            var result = new JsonData();

            if (ret.return_code == "FAIL")
            {
                result.Msg = ret.return_msg;
            }
            else if (ret.result_code == "FAIL")
            {
                result.Msg = ret.err_code_des;
            }
            else
            {
                result.Success = true;
                var payment = new WxPayment
                {
                    appId = business.WeChatAppId,
                    package = "prepay_id=" + ret.prepay_id,
                    key = business.PayServerKey
                };
                // 保存支付标识码
                record.PrepayId = ret.prepay_id;
                Service.Commit();
                payment.Generator();
                result.Data = payment;
            }
            return Json(result);
        }

        /// <summary>
        /// 支付成功回调（微信调用）
        /// </summary>
        /// <returns></returns>
        [HttpPost("paySuccess")]
        public async Task<IActionResult> PaySuccess()
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var content = sr.ReadToEnd();
                Log.Debug(content);
                var ret = UtilHelper.ReadXml<WxPaySuccess>(content);
                if (string.IsNullOrEmpty(ret.transaction_id)) return BadRequest("支付不成功");
                var record = await Service.ChargeSuccessAsync(ret);
            }
            return Content("SUCCESS");
        }

        /// <summary>
        /// 获取会员充值记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("records")]
        public async Task<IActionResult> GetChargeRecords([FromQuery]int id, [FromQuery]PagingQuery paging)
        {
            return Json(await Service.GetChargeRecordsAsync(id, paging));
        }

    }
}