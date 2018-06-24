using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class BusinessController : BaseController<IBusinessRepository, Business>
    {
        public BusinessController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }
        /// <summary>
        /// 商户基本信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 小程序信息
        /// </summary>
        /// <returns></returns>
        public IActionResult SmallProgram()
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 达达配置
        /// </summary>
        /// <param name="cityList"></param>
        /// <returns></returns>
        public IActionResult Dada([FromServices]List<City> cityList)
        {
            ViewBag.cityList = JsonConvert.SerializeObject(cityList.Select(a => new { a.Name, a.Code }), AppData.JsonSetting);
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 飞印打印机配置
        /// </summary>
        /// <returns></returns>
        public IActionResult Feyin()
        {
            var printers = Service.GetPrinters(Business.ID);
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            ViewBag.printers = JsonConvert.SerializeObject(printers, AppData.JsonSetting);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveBase([FromQuery]int isChangeLogo, [FromBody]Business business)
        {
            var result = new JsonData();

            if (string.IsNullOrEmpty(business.LogoSrc))
            {
                result.Msg = "请上传商户LOGO";
                return Ok(result);
            }
            if (isChangeLogo == 1)
            {
                // 上传LOGO
                var filename = Guid.NewGuid().ToString().ToLower() + ".jpg";
                var msg = await Service.UploadLogoAsync(AppData.ApiUri + "/Logo", Business.ID, filename, business.LogoSrc);
                if (msg != "ok")
                {
                    result.Msg = msg;
                    return Json(result);
                }
                business.LogoSrc = filename;
            }


            result.Success = Service.SaveBase(business);
            if (!result.Success)
            {
                result.Msg = "保存失败";
            }
            Business.Name = business.Name;
            Business.Email = business.Email;
            Business.Address = business.Address;
            Business.Contact = business.Contact;
            Business.Mobile = business.Mobile;
            Business.IsAutoReceipt = business.IsAutoReceipt;
            Business.Freight = business.Freight;
            Business.Description = business.Description;
            Business.Range = business.Range;
            Business.LogoSrc = business.LogoSrc;
            Business.BusinessLicense = business.BusinessLicense;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }

        public IActionResult SaveSmall([FromBody]Business business)
        {
            var result = new JsonData
            {
                Success = Service.SaveSmall(new Business { ID = Business.ID, AppId = business.AppId, Secret = business.Secret })
            };
            if (!result.Success)
            {
                result.Msg = "保存失败";
            }
            Business.AppId = business.AppId;
            Business.Secret = business.Secret;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }

        public IActionResult SaveDada([FromBody]Business business)
        {
            business.ID = Business.ID;
            var result = new JsonData
            {
                Success = Service.SaveDada(business)
            };
            if (!result.Success)
            {
                result.Msg = "保存失败，请刷新后重试";
            }
            Business.DadaAppKey = business.DadaAppKey;
            Business.DadaAppSecret = business.DadaAppSecret;
            Business.DadaSourceId = business.DadaSourceId;
            Business.DadaShopNo = business.DadaShopNo;
            Business.CityCode = business.CityCode;
            Business.CityName = business.CityName;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }

        public IActionResult SaveFeyin([FromQuery]Business business)
        {
            business.ID = Business.ID;
            var result = new JsonData
            {
                Success = Service.SaveFeyin(business)
            };
            if (!result.Success)
            {
                result.Msg = "保存失败，请刷新后重试";
            }
            Business.FeyinApiKey = business.FeyinApiKey;
            Business.FeyinMemberCode = business.FeyinMemberCode;
            HttpContext.Session.Set(AppData.Session, Business);
            result.Msg = "修改成功";
            return Ok(result);
        }

        /// <summary>
        /// 绑定打印机
        /// </summary>
        /// <param name="device"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddBind([FromQuery]FeyinDevice device, [FromServices]FeYinHelper helper)
        {
            var result = new JsonData();
            if(Business.FeyinMemberCode == helper.MemberCode)
            {
                // 如果商户使用的是系统注册的商户，则直接采用服务中注册的类绑定设备
                var ret = await helper.BindDevice(device.Code);
                result.Success = ret.ErrCode == null || ret.ErrCode == 0;
                result.Msg = ret.ErrMsg;
            }
            else
            {
                // 否则创建一个服务类
                var util = new FeYinHelper
                {
                    MemberCode = Business.FeyinMemberCode,
                    ApiKey = Business.FeyinApiKey,
                    Token = Business.FeyinToken
                };

                var ret = await util.BindDevice(device.Code);
                result.Success = ret.ErrCode == null || ret.ErrCode == 0;
                result.Msg = ret.ErrMsg;

                if (Business.FeyinToken != util.Token)
                {
                    // 如果商户Session中保存的令牌与执行打印后的Token不一致，则修改商户中的Token
                    Business.FeyinToken = util.Token;
                    HttpContext.Session.Set(AppData.Session, Business);
                }
            }
            if (!result.Success)
            {
                // 绑定不成功
                return Json(result);
            }
            result.Success = Service.BindPrintDevice(Business, device);
            if(!result.Success)
            {
                result.Msg = "绑定失败，请刷新后重试";
            }
            result.Data = device;
            result.Msg = "设备绑定成功";
            return Json(result);
        }

    }
}
