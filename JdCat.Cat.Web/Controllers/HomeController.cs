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

namespace JdCat.Cat.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(AppData appData) : base(appData)
        {
        }

        public IActionResult Index()
        {
            return View(Business);
        }

        public async Task<IActionResult> About([FromServices]IOptions<AppData> appdata)
        {
            ViewData["Message"] = "Your application description page.";
            var client = new HttpClient();
            var data = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "user", "sunxsh" },
                { "pwd", "000000"}
            });
            var result = client.GetAsync(appdata.Value.ApiUri + "/business?user=sunxsh&pwd=000000");
            var stream = result.Result.Content.ReadAsStringAsync();
            var a = await stream;
            client.Dispose();
            ViewBag.Msg = a;
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
