using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
namespace JdCat.Cat.Web.Controllers
{
    public class RegisteController : Controller
    {
        private AppData appData;
        private IBusinessRepository service;
        public RegisteController(AppData appData, IBusinessRepository rep)
        {
            this.appData = appData;
            this.service = rep;
        }

        public IActionResult Index()
        {
            ViewBag.CompanyName = appData.Name;
            return View();
        }

        public IActionResult Registe(
            string name,
            string pwd,
            string code,
            string invitationCode,
            string email,
            string address,
            string phone,
            string mark,
            string license)
        {
            var result = new JsonData();
            if (service.Exists(a => (a.Name == name)))
            {
                result.Msg = "该商户已注册";
                result.Success = false;
                return Json(result);
            }
            else if (service.Exists(a => (a.Code == code)))
            {
                result.Msg = "该用户名已注册";
                result.Success = false;
                return Json(result);
            }
            //else if ("168168168" != invitationCode)
            //{
            //    result.Msg = "该邀请码无效";
            //    result.Success = false;
            //    return Json(result);
            //}
            //else if (service.Exists(a => (a.InvitationCode == invitationCode)))
            //{
            //    result.Msg = "该邀请码已被使用";
            //    result.Success = false;
            //    return Json(result);
            //}
            else
            {
                var userInfo = new Business()
                {
                    Name = name,
                    Password = UtilHelper.MD5Encrypt(pwd),
                    Code = code,
                    Email = email,
                    InvitationCode = invitationCode,
                    Address = address,
                    Mobile = phone,
                    BusinessLicense = license,
                    Description = mark,
                    Freight = 4,
                    BusinessStartTime = "06:00",
                    BusinessEndTime = "21:00",
                    Category = BusinessCategory.Store,
                    ObjectId = Guid.NewGuid().ToString().ToLower()
                };
                userInfo.FeyinMemberCode = appData.FeyinMemberCode;
                userInfo.FeyinApiKey = appData.FeyinApiKey;
                service.Add(userInfo);
                result.Msg = "注册成功";
                result.Success = true;
                return Json(result);
            }
        }

        public IActionResult R2()
        {
            ViewBag.CompanyName = appData.Name;
            return View();
        }

        [HttpPost]
        public IActionResult Registe2([FromBody]Business business)
        {
            var result = new JsonData();
            var isExist = service.ExistForCode(business.Code);
            if (isExist)
            {
                result.Msg = "登录帐号已存在，请修改后提交";
                return Json(result);
            }
            business.ObjectId = Guid.NewGuid().ToString().ToLower();
            business.Password = UtilHelper.MD5Encrypt(business.Password);
            business.Category = BusinessCategory.Chain;
            business.RegisterDate = DateTime.Now;
            service.Add(business);
            result.Msg = "连锁店总后台注册成功";
            result.Success = true;
            return Json(result);
        }

        public IActionResult Login()
        {
            HttpContext.Session.SetString(appData.Session, "");
            HttpContext.Response.Cookies.Delete(appData.Cookie);
            return Redirect("/Login");
        }
    }
}
