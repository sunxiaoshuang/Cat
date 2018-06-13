using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class BusinessController : Controller
    {
        public IActionResult Index([FromServices]List<City> cityList)
        {
            ViewBag.cityList = JsonConvert.SerializeObject(cityList);
            return View();
        }
    }
}