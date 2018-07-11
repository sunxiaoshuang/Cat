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
            Business.BusinessLicenseImage = business.BusinessLicenseImage;
            Business.Lng = business.Lng;
            Business.Lat = business.Lat;
            HttpContext.Session.Set(AppData.Session, Business);
            return Ok(result);
        }

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
        public async Task<IActionResult> AddBind([FromQuery]FeyinDevice device)
        {
            var result = new JsonData();
            var helper = GetPrintHelper();

            var ret = await helper.BindDevice(device.Code);
            result.Success = ret.ErrCode == null || ret.ErrCode == 0;
            result.Msg = ret.ErrMsg;

            if (Business.FeyinToken != helper.Token)
            {
                // 如果商户Session中保存的令牌与执行打印后的Token不一致，则修改商户中的Token
                Business.FeyinToken = helper.Token;
                HttpContext.Session.Set(AppData.Session, Business);
            }


            if (!result.Success)
            {
                // 绑定不成功
                return Json(result);
            }
            result.Success = Service.BindPrintDevice(Business, device);
            if (!result.Success)
            {
                result.Msg = "绑定失败，请刷新后重试";
            }
            result.Data = device;
            result.Msg = "设备绑定成功";
            return Json(result);
        }
        /// <summary>
        /// 绑定打印机
        /// </summary>
        /// <param name="device"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> UnBind(int id)
        {
            var result = new JsonData();
            var device = Service.Set<FeyinDevice>().SingleOrDefault(a => a.ID == id);
            if (device == null)
            {
                result.Msg = "设备可以已经被删除，请刷新后重试";
                return Json(result);
            }
            var helper = GetPrintHelper();

            var ret = await helper.UnBindDevice(device.Code);
            result.Success = ret.ErrCode == null || ret.ErrCode == 0;
            result.Msg = ret.ErrMsg;

            if (Business.FeyinToken != helper.Token)
            {
                // 如果商户Session中保存的令牌与执行打印后的Token不一致，则修改商户中的Token
                Business.FeyinToken = helper.Token;
                HttpContext.Session.Set(AppData.Session, Business);
            }

            if (!result.Success)
            {
                // 解绑失败不成功
                return Json(result);
            }

            Service.Set<FeyinDevice>().Remove(device);
            result.Success = Service.Commit() > 0;
            if (!result.Success)
            {
                result.Msg = "解除绑定失败，请刷新后重试";
            }
            result.Msg = "解除绑定绑定成功";
            return Json(result);
        }

        public IActionResult Pwd()
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, AppData.JsonSetting);
            return View();
        }

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

    }
}
