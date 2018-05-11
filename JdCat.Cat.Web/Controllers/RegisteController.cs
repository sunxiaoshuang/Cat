﻿using System;
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
            string license,
            [FromServices]UtilHelper helper)
        {
            var result = new JsonData();
            using (var client = new HttpClient())
            {
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
                else if ("168168168" != invitationCode)
                {
                    result.Msg = "该邀请码无效";
                    result.Success = false;
                    return Json(result);
                }
                else if (service.Exists(a => (a.InvitationCode == invitationCode)))
                {
                    result.Msg = "该邀请码已被使用";
                    result.Success = false;
                    return Json(result);
                }
                else
                {
                    var userInfo = new Business() {
                        Name = name,
                        Password = helper.GetMd5(pwd),
                        Code = code,
                        Email = email,
                        InvitationCode = invitationCode,
                        Address = address,
                        Mobile = phone,
                        BusinessLicense = license,
                        Description = mark
                    };
                    service.Add(userInfo);
                    result.Msg = "注册成功";
                    result.Success = true;
                    return Json(result);
                }
            }
        }

        public IActionResult Login()
        {
            HttpContext.Session.SetString(appData.Session, "");
            HttpContext.Response.Cookies.Delete(appData.Cookie);
            return Redirect("/Login");
        }
    }
}
