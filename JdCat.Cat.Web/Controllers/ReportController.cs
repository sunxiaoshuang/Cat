using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web.Controllers
{
    public class ReportController : BaseController<IBusinessRepository, Business>
    {
        public ReportController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            var time = DateTime.Now;
            var orders = Service.GetOrderTotal(Business, time.AddDays(-6), time.AddDays(1));
            for (int i = 0; i < 7; i++)
            {
                var cur = time.AddDays(-i);
                var order = orders.FirstOrDefault(a => a.CreateTime == cur.ToString("yyyy-MM-dd"));
                if (order != null) continue;
                orders.Add(new Report_Order { CreateTime = cur.ToString("yyyy-MM-dd"), Price = 0, Quantity = 0 });
            }
            orders = orders.OrderBy(a => a.CreateTime).ToList();

            ViewBag.list = JsonConvert.SerializeObject(orders, AppData.JsonSetting);
            ViewBag.products = JsonConvert.SerializeObject(Service.GetProductTotal(Business, time), AppData.JsonSetting);
            return View();
        }

        


    }
}