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
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.Web.Controllers
{
    public class HomeController : BaseController<IBusinessRepository, Business>
    {
        public HomeController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index([FromServices]JsonSerializerSettings setting)
        {
            if(Business.Category == BusinessCategory.Chain)
            {
                return RedirectToAction("Index", "Chain");
            }
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
    }
}
