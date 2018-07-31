using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class SaleController : BaseController<IBusinessRepository, Business>
    {
        public SaleController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FullReduceList()
        {
            var list = Service.GetFullReduce(Business).ToList();
            ViewBag.list = JsonConvert.SerializeObject(list, AppData.JsonSetting);
            return View();
        }

        public IActionResult FullReduce(int? id)
        {
            if(id != null)
            {
                ViewBag.entity = JsonConvert.SerializeObject(Service.GetFullReduceById(id.Value), AppData.JsonSetting);
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateFullReduce([FromBody]SaleFullReduce entity)
        {
            entity.BusinessId = Business.ID;
            return Json(Service.CreateFullReduce(entity));
        }
        [HttpPost]
        public IActionResult UpdateFullReduce([FromBody]SaleFullReduce entity)
        {
            return Json(Service.UpdateFullReduce(entity));
        }

        public IActionResult DeleteFullReduce(int id)
        {
            return Json(Service.DeleteFullReduce(id));
        }

        public IActionResult Coupon()
        {
            return View();
        }

        public IActionResult CouponList()
        {
            return View();
        }

    }
}