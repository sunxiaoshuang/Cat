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

namespace JdCat.Cat.Web.Controllers
{
    /// <summary>
    /// 控制器会员卡
    /// </summary>
    public class MemberController : Controller
    {

        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
            }
        }

        public async Task<IActionResult> Index([FromQuery]string card_id, [FromQuery]string encrypt_code, [FromQuery]string openid, [FromServices]ICardRepository service)
        {
            var member = await service.GetMemberAsync(card_id, openid);
            if (member == null) return Content("会员不存在");
            var card = await service.GetCardAsync(card_id);
            ViewBag.color = WxHelper.WxColors[card.Color];
            ViewBag.card_id = card_id;
            ViewBag.encrypt_code = encrypt_code;
            ViewBag.openid = openid;
            return View(member);
        }

        /// <summary>
        /// 获取支付码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPayCode(int id, [FromServices]ICardRepository service)
        {
            var payment = await service.CreatePaymentTargetAsync(id);
            return Content(payment.Code);
        } 

        /// <summary>
        /// 获取支付二维码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IActionResult GetPayEwm([FromQuery]string code)
        {
            var img = UtilHelper.CreateCodeEwm(code, 231, 231);
            return File(img, "image/jpeg");
        }

        /// <summary>
        /// 获取支付条形码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IActionResult GetPayTxm([FromQuery]string code)
        {
            var img = UtilHelper.CreateCodeTxm(code);
            return File(img, "image/jpeg");
        }

        /// <summary>
        /// 获取支付结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPaymentResult([FromQuery]string code, [FromServices]ICardRepository service)
        {
            var payment = await service.GetPaymentTargetAsync(code);
            return Json(payment, AppData.JsonSetting);
        }

        /// <summary>
        /// 会员充值
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Recharge([FromQuery]string card_id, [FromQuery]string encrypt_code, [FromQuery]string openid, [FromServices]ICardRepository service)
        {
            var member = await service.GetMemberAsync(card_id, openid);
            if (member == null) return Content("会员不存在");
            var card = await service.GetCardAsync(card_id);
            ViewBag.color = WxHelper.WxColors[card.Color];
            ViewBag.card_id = card_id;
            ViewBag.encrypt_code = encrypt_code;
            ViewBag.openid = openid;

            return View(member);
        }

        /// <summary>
        /// 充值记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="card_id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RechargeRecord(int id, [FromServices]ICardRepository service)
        {
            var member = await service.GetAsync<WxMember>(id);
            var card = await service.GetAsync<WxCard>(member.WxCardId);
            ViewBag.color = WxHelper.WxColors[card.Color];

            return View();
        }

    }
}
