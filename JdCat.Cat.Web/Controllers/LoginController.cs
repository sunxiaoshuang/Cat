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
            return View();
        }

        public IActionResult Login(string username, string pwd, [FromServices]UtilHelper helper)
        {
            var result = new JsonData();
            var business = _service.Get(a => (a.Code == username || a.Mobile == username) && a.Password == helper.MD5Encrypt(pwd));
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