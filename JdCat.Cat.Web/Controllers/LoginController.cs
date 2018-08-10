using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppData _appData;
        private readonly IBusinessRepository _service;
        public LoginController(AppData appData, IBusinessRepository rep)
        {
            this._appData = appData;
            this._service = rep;
        }

        public IActionResult Index()
        {
            ViewBag.CompanyName = _appData.Name;
            ViewBag.type = Request.Query["type"];
            ViewBag.isVerifyCode = HttpContext.Session.Get<string>("VerificationCode") != null;
            return View();
        }

        public IActionResult Code()
        {
            var random = new Random();
            var code = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                code += random.Next(0, 9);
            }
            HttpContext.Session.Set("VerificationCode", code);
            byte[] buffer = UtilHelper.GenerateVerificationCode(code);
            return File(buffer, "image/png");
        }

        public IActionResult Login(string username, string pwd, string code)
        {
            var result = new JsonData();
            if(code != null)
            {
                if(HttpContext.Session.Get<string>("VerificationCode") != code)
                {
                    result.Msg = "验证码错误";
                    return Json(result);
                }
            }
            var business = _service.GetBusiness(a => (a.Code == username || a.Mobile == username) && a.Password == UtilHelper.MD5Encrypt(pwd));
            if (business == null)
            {
                result.Msg = "帐号或密码错误";
                return Json(result);
            }
            result.Success = true;
            result.Data = business;
            HttpContext.Session.Set(_appData.Session, business);
            HttpContext.Response.Cookies.Append(_appData.Cookie, business.ID.ToString(),
                new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(3)
                });
            return Json(result);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString(_appData.Session, "");
            HttpContext.Response.Cookies.Delete(_appData.Cookie);
            return Redirect("/Login");
        }
    }
}