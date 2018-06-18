using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class BusinessController : BaseController<IBusinessRepository, Business>
    {
        public BusinessController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index([FromServices]JsonSerializerSettings setting)
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, setting);
            return View();
        }

        public IActionResult SmallProgram([FromServices]JsonSerializerSettings setting)
        {
            ViewBag.business = JsonConvert.SerializeObject(Business, setting);
            return View();
        }

        public IActionResult Dada([FromServices]List<City> cityList, [FromServices]JsonSerializerSettings setting)
        {
            ViewBag.cityList = JsonConvert.SerializeObject(cityList.Select(a => new { a.Name, a.Code }), setting);
            ViewBag.business = JsonConvert.SerializeObject(Business, setting);
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
                result.Msg = "保存失败";
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

    }
}
