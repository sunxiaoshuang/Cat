using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index([FromServices]AppData appData)
        {
            ViewBag.CompanyName = appData.Name;
            return View();
        }

        public async Task<IActionResult> Login(string username, string pwd, [FromServices]AppData appData)
        {
            var result = new JsonData();
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{appData.ApiUri}/Business?user={username}&pwd={pwd}");
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                {
                    result.Msg = "帐号或密码错误";
                    return Json(result);
                }
                result.Success = true;
                var business = JsonConvert.DeserializeObject<Business>(content);
                result.Data = business;
                HttpContext.Session.Set("User_Sesssion", business);
                return Json(result);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Set("User_Sesssion", "");
            return Redirect("/Login");
        }
    }
}