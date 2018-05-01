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
        private AppData appData;
        private IBusinessRepository service;
        public LoginController(AppData appData, IBusinessRepository rep)
        {
            this.appData = appData;
            this.service = rep;
        }

        public IActionResult Index()
        {
            ViewBag.CompanyName = appData.Name;
            return View();
        }

        public IActionResult Login(string username, string pwd, [FromServices]UtilHelper helper)
        {
            var result = new JsonData();
            using (var client = new HttpClient())
            {
                var business = service.Get(a => (a.Code == username || a.Mobile == username) && a.Password == helper.GetMd5(pwd));
                if(business == null)
                {
                    result.Msg = "帐号或密码错误";
                    return Json(result);
                }
                result.Success = true;
                result.Data = business;
                HttpContext.Session.Set(appData.Session, business);
                HttpContext.Response.Cookies.Append(appData.Cookie, business.ID.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7)
                    });
                return Json(result);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString(appData.Session, "");
            HttpContext.Response.Cookies.Delete(appData.Cookie);
            return Redirect("/Login");
        }
    }
}