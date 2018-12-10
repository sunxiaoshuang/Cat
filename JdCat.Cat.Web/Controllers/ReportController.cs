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

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
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
            ViewBag.products = JsonConvert.SerializeObject(Service.GetProductTop10(Business, time), AppData.JsonSetting);
            ViewBag.productPrices = JsonConvert.SerializeObject(Service.GetProductPriceTop10(Business, time), AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 销售统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SaleStatistics()
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
            return View();
        }

        /// <summary>
        /// 营业统计页
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSaleStatistics([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
            {
                return Json(new string[] { });
            }
            return Json(Service.GetSaleStatistics(Business, start.Value, end.Value));
        }

        /// <summary>
        /// 导出营业统计
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExportSaleStatistics([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            var name = $"销售统计({start.Value:yyyyMMdd}-{end.Value:yyyyMMdd}).xlsx";
            var list = Service.GetSaleStatistics(Business, start.Value, end.Value);
            var index = 1;
            var totalPrice = 0d;
            var totalQuantity = 0;
            list.ForEach(a =>
            {
                a.Index = index++;
                totalPrice += a.Total;
                totalQuantity += a.Quantity;
            });
            list.Add(new Report_SaleStatistics { Index = index, Date = "合计", Total = totalPrice, Quantity = totalQuantity });

            return File(list.ToWorksheet(), AppData.XlsxContentType, name);
        }

        /// <summary>
        /// 商品分析页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProductAnalysis()
        {
            ViewBag.products = JsonConvert.SerializeObject(Service.GetProductTop10(Business, DateTime.Now), AppData.JsonSetting);
            ViewBag.productPrices = JsonConvert.SerializeObject(Service.GetProductPriceTop10(Business, DateTime.Now), AppData.JsonSetting);
            return View();
        }


    }
}