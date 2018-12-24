using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
            Business.FeyinApiKey = "";
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            ViewBag.printers = JsonConvert.SerializeObject(printers, AppData.JsonSetting);
            return View();
        }
        /// <summary>
        /// 保存商户基本信息
        /// </summary>
        /// <param name="isChangeLogo"></param>
        /// <param name="business"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveBase([FromQuery]int isChangeLogo, [FromBody]Business business)
        {
            var result = new JsonData();
            if (isChangeLogo == 1)
            {
                // 上传LOGO
                business.LogoSrc = await Service.UploadAsync(AppData.ApiUri + "/Upload/Logo", Business.ID, business.LogoSrc);
            }
            if (business.BusinessLicenseImage.Contains("data:image"))
            {
                // 上传营业执照
                business.BusinessLicenseImage = await Service.UploadAsync(AppData.ApiUri + "/Upload/License", Business.ID, business.BusinessLicenseImage);
            }
            if (!string.IsNullOrEmpty(business.SpecialImage) && business.SpecialImage.Contains("data:image"))
            {
                // 上传特殊资质
                business.SpecialImage = await Service.UploadAsync(AppData.ApiUri + "/Upload/License", Business.ID, business.SpecialImage);
            }


            result.Success = Service.SaveBase(business);
            if (!result.Success)
            {
                result.Msg = "保存失败";
                return Json(result);
            }
            Business.Name = business.Name;
            Business.Email = business.Email;
            Business.Address = business.Address;
            Business.Contact = business.Contact;
            Business.Mobile = business.Mobile;
            Business.FreightMode = business.FreightMode;
            Business.Freight = business.Freight;
            Business.Description = business.Description;
            Business.Range = business.Range;
            Business.LogoSrc = business.LogoSrc;
            Business.BusinessLicense = business.BusinessLicense;
            Business.BusinessLicenseImage = business.BusinessLicenseImage;
            Business.SpecialImage = business.SpecialImage;
            Business.Lng = business.Lng;
            Business.Lat = business.Lat;
            Business.BusinessStartTime = business.BusinessStartTime;
            Business.BusinessEndTime = business.BusinessEndTime;
            Business.BusinessStartTime2 = business.BusinessStartTime2;
            Business.BusinessEndTime2 = business.BusinessEndTime2;
            Business.BusinessStartTime3 = business.BusinessStartTime3;
            Business.BusinessEndTime3 = business.BusinessEndTime3;
            Business.MinAmount = business.MinAmount;
            Business.ServiceProvider = business.ServiceProvider;
            Business.Province = business.Province;
            Business.City = business.City;
            Business.Area = business.Area;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }
        /// <summary>
        /// 更改店铺接单状态
        /// </summary>
        /// <param name="isAutoReceipt"></param>
        /// <returns></returns>
        public IActionResult ChangeAutoReceipt([FromQuery]bool isAutoReceipt)
        {
            var result = new JsonData
            {
                Success = Service.ChangeAutoReceipt(Business, isAutoReceipt)
            };
            Business.IsAutoReceipt = isAutoReceipt;
            if (result.Success)
            {
                HttpContext.Session.Set(AppData.Session, Business);
                result.Msg = "操作成功";
            }
            else
            {
                result.Msg = "操作失败";
            }
            return Json(result);
        }
        /// <summary>
        /// 更改店铺营业状态
        /// </summary>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public IActionResult ChangeClose([FromQuery]bool isClose)
        {
            var result = new JsonData
            {
                Success = Service.ChangeClose(Business, isClose)
            };
            Business.IsClose = isClose;
            if (result.Success)
            {
                HttpContext.Session.Set(AppData.Session, Business);
                result.Msg = "操作成功";
            }
            else
            {
                result.Msg = "操作失败";
            }
            return Json(result);
        }
        /// <summary>
        /// 保存小程序设置信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        public IActionResult SaveSmall([FromBody]Business business)
        {
            var result = new JsonData
            {
                Success = Service.SaveSmall(business)
            };
            if (!result.Success)
            {
                result.Msg = "保存失败";
            }
            Business.AppId = business.AppId;
            Business.Secret = business.Secret;
            Business.MchId = business.MchId;
            Business.MchKey = business.MchKey;
            Business.TemplateNotifyId = business.TemplateNotifyId;
            Business.AppQrCode = business.AppQrCode;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }
        /// <summary>
        /// 保存达达设置信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
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
            //Business.DadaAppKey = business.DadaAppKey;
            //Business.DadaAppSecret = business.DadaAppSecret;
            Business.DadaSourceId = business.DadaSourceId;
            Business.DadaShopNo = business.DadaShopNo;
            Business.CityCode = business.CityCode;
            Business.CityName = business.CityName;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }
        /// <summary>
        /// 保存飞印用户信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
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
        public async Task<IActionResult> AddBind([FromQuery]FeyinDevice device)
        {
            var result = new JsonData();
            result.Success = await Service.BindPrintDeviceAsync(Business, device);
            if (!result.Success)
            {
                result.Msg = "绑定失败，请刷新后重试";
            }
            result.Data = device;
            result.Msg = "设备绑定成功";
            return Json(result);
        }
        /// <summary>
        /// 解绑打印机
        /// </summary>
        /// <param name="device"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> UnBind(int id)
        {
            var result = await Service.UnbindPrintDeviceAsync(id);
            return Json(result);
        }
        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Pwd()
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            return View();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="oldPwd"></param>
        /// <returns></returns>
        public IActionResult ModifyPassword([FromQuery]string newPwd, [FromQuery]string oldPwd)
        {
            var result = new JsonData();
            if (Business.Password != UtilHelper.MD5Encrypt(oldPwd))
            {
                result.Msg = "原始密码错误";
                return Json(result);
            }
            Business.Password = UtilHelper.MD5Encrypt(newPwd);
            result.Success = Service.UpdatePassword(Business);
            if (!result.Success)
            {
                result.Msg = "密码修改错误，请刷新后重试";
                return Json(result);
            }
            HttpContext.Session.Set(AppData.Session, Business);

            result.Msg = "修改成功";
            return Json(result);
        }
        /// <summary>
        /// 设置默认打印机
        /// </summary>
        /// <returns></returns>
        public IActionResult SetDefaultPrinter(int id)
        {
            var result = new JsonData();
            result.Success = Service.SetDefaultPrinter(Business, id);

            if (!result.Success)
            {
                result.Msg = "设置失败，请刷新后重试";
                return Json(result);
            }
            result.Msg = "设置成功";
            return Json(result);
        }
        /// <summary>
        /// 消息管理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Message()
        {
            if (string.IsNullOrEmpty(Business.WxQrListenPath))
            {
                var token = await WxHelper.GetTokenAsync(WxHelper.WeChatAppId, WxHelper.WeChatSecret);
                var ticket = await WxHelper.GetTicketAsync(Business.ID, token);
                var qrCode = UtilHelper.CreateCodeEwm(ticket.url);
                var source = Convert.ToBase64String(qrCode);
                Business.WxQrListenPath = await Service.UploadAsync(AppData.ApiUri + "/Upload/Qrcode", Business.ID, source);
                Service.SaveWxQrcode(Business);
                SaveSession();
            }
            ViewBag.list = Service.GetWxListenUser(Business.ID);
            return View(Business);
        }
        /// <summary>
        /// 删除微信通知用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteWxUser(int id)
        {
            Service.RemoveWxListenUser(id);
            return Content("删除成功");
        }

        /// <summary>
        /// 运费设置
        /// </summary>
        /// <returns></returns>
        public IActionResult Freight()
        {
            ViewBag.Freights = JsonConvert.SerializeObject(Service.GetFreights(Business.ID), AppData.JsonSetting);
            return View();
        }

        public IActionResult CreateFreight([FromBody]BusinessFreight freight)
        {
            freight.BusinessId = Business.ID;
            var entity = Service.CreateFreight(freight);
            var result = new JsonData { Success = true, Data = entity, Msg = "新增成功" };
            return Json(result);
        }
        public IActionResult UpdateFreight([FromBody]BusinessFreight freight)
        {
            var entity = Service.UpdateFreight(freight);
            var result = new JsonData { Success = true, Data = entity, Msg = "修改成功" };
            return Json(result);
        }
        public IActionResult RemoveFreight(int id)
        {
            var flag = Service.RemoveFreight(id);
            var result = new JsonData { Success = flag, Msg = flag ? "删除成功" : "删除失败，记录不存在或已删除，请刷新页面后再试" };
            return Json(result);
        }


        public IActionResult OpenSetting()
        {
            return View();
        }

        public async Task<IActionResult> PreAuthCode([FromServices]AppData appData)
        {
            var result = new JsonData();
            var preCode = await WxHelper.GetOpenPreAuthCodeAsync(appData);
            if (preCode == null)
            {
                result.Msg = "获取预授权码失败，请稍后再试";
            }
            else
            {
                result.Success = true;
                result.Data = $"https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid={appData.OpenAppId}&pre_auth_code={preCode}&redirect_uri=http://t.e.jiandanmao.cn&auth_type=3";
            }
            return Json(result);
        }

        /// <summary>
        /// 重定向到授权页
        /// </summary>
        /// <param name="appData"></param>
        /// <returns></returns>
        public async Task<IActionResult> AuthPage([FromServices]AppData appData)
        {
            var preCode = await WxHelper.GetOpenPreAuthCodeAsync(appData);
            var url = $"https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid={appData.OpenAppId}&pre_auth_code={preCode}&redirect_uri=http://t.e.jiandanmao.cn/Business/AuthSuccess&auth_type=3";
            return Redirect(url);
        }
        
        /// <summary>
        /// 授权成功后的回调URL
        /// </summary>
        /// <param name="auth_code"></param>
        /// <param name="expires_in"></param>
        /// <returns></returns>
        public async Task<IActionResult> AuthSuccess([FromQuery]string auth_code, [FromQuery]int expires_in, [FromServices]AppData appData)
        {
            var result = await WxHelper.GetAuthToken(appData, auth_code);
            return Json(result);
        }

        public IActionResult SetCode([FromQuery]string code)
        {
            if (string.IsNullOrEmpty(WxHelper.OpenTicket))
            {
                WxHelper.OpenTicket = code;
            }
            return Content("ok");
        }

    }
}
