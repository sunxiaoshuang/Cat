using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JdCat.Cat.Web.Models;
using JdCat.Cat.Common;
using Microsoft.Extensions.Options;
using JdCat.Cat.Model.Data;
using JdCat.Cat.IRepository;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class HomeController : BaseController<IBusinessRepository, Business>
    {
        public HomeController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index([FromServices]JsonSerializerSettings setting)
        {
            ViewBag.entity = JsonConvert.SerializeObject(Business, setting);
            ViewBag.orderUrl = AppData.OrderUrl;
            ViewBag.UserName = Business.Name;
            ViewBag.Code = Business.Code;
            ViewBag.CompanyName = AppData.Name;
            return View();
        }

        public IActionResult Empty()
        {
            return View();
        }

        public IActionResult About([FromServices]AppData appdata)
        {
            //ViewData["Message"] = "Your application description page.";
            //var client = new HttpClient();
            //var data = new FormUrlEncodedContent(new Dictionary<string, string>()
            //{
            //    { "user", "sunxsh" },
            //    { "pwd", "000000"}
            //});
            //var result = client.GetAsync(appdata.ApiUri + "/business?user=sunxsh&pwd=000000");
            //var stream = result.Result.Content.ReadAsStringAsync();
            //var a = await stream;
            //client.Dispose();
            //ViewBag.Msg = a;
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
