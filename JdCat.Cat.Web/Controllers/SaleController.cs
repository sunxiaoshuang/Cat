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
            if (id != null)
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

        public IActionResult Coupon(int? id)
        {
            if (id != null)
            {
                ViewBag.entity = JsonConvert.SerializeObject(Service.GetCouponById(id.Value), AppData.JsonSetting);
            }
            return View();
        }

        public IActionResult CouponList()
        {
            ViewBag.list = JsonConvert.SerializeObject(Service.GetCoupon(Business).ToList(), AppData.JsonSetting);
            return View();
        }

        public IActionResult CreateCoupon([FromBody]SaleCoupon coupon)
        {
            coupon.BusinessId = Business.ID;
            return Json(Service.CreateCoupon(coupon));
        }

        public IActionResult DeleteCoupon(int id)
        {
            return Json(Service.DeleteCoupon(id));
        }

        public IActionResult DownCoupon(int id)
        {
            return Json(Service.DownCoupon(id));
        }

        public IActionResult Discount([FromServices]IProductRepository repository)
        {
            ViewBag.discountList = JsonConvert.SerializeObject(Service.GetDiscounts(Business), AppData.JsonSetting);
            var typeList = repository.GetTypes(Business, Model.Enum.ProductStatus.Sale).ToList();
            typeList.ForEach(type =>
            {
                var removeList = new List<Product>();
                foreach (var item in type.Products)
                {
                    if (item.Formats.Count > 1)
                    {
                        removeList.Add(item);
                    }
                    //if(item.Attributes.Count > 0)
                    //{
                    //    removeList.Add(item);
                    //}
                }
                removeList.ForEach(a => type.Products.Remove(a));
            });
            ViewBag.typeList = JsonConvert.SerializeObject(typeList, AppData.JsonSetting);
            return View();
        }

        public IActionResult DiscountProduct()
        {
            return View();
        }
        public IActionResult DiscountDetail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDiscount([FromBody]List<SaleProductDiscount> list)
        {
            list.ForEach(a => a.BusinessId = Business.ID);
            list = Service.CreateDiscount(list);
            return Json(list);
        }

        [HttpDelete]
        public IActionResult DeleteDiscount(int id)
        {
            return Json(Service.DeleteDiscount(id));
        }

        [HttpPost]
        public IActionResult UpdateDiscount([FromBody]SaleProductDiscount discount)
        {
            var result = Service.UpdateDiscount(discount);
            result.Msg = result.Success ? "修改成功" : "修改失败";
            return Json(result);
        }

        public async Task<IActionResult> NewCustom()
        {
            var entity = await Service.GetBusinessNewCustomAsync(Business.ID);
            return View(entity ?? new SaleNewCustom { StartTime = DateTime.Now, EndTime = DateTime.Now.AddMonths(1) });
        }

        public async Task<IActionResult> SaveNewCustom([FromBody]SaleNewCustom entity)
        {
            if (entity.ID > 0)
            {
                await Service.UpdateAsync(entity, new[] { nameof(entity.Amount), nameof(entity.StartTime), nameof(entity.EndTime) });
            }
            else
            {
                entity.BusinessId = Business.ID;
                await Service.AddAsync(entity);
            }
            return Content("ok");
        }

        [HttpGet]
        public IActionResult SaleReturn()
        {
            return View();
        }

        public async Task<IActionResult> GetReturnList()
        {
            return Json(await Service.GetBusinessReturnCouponAsync(Business.ID));
        }

        [HttpPost]
        public async Task<IActionResult> SaveReturnCoupon([FromBody]SaleReturnCoupon coupon)
        {
            coupon.Name = coupon.Value + "元优惠券";
            coupon.BusinessId = Business.ID;
            if (coupon.MinConsume < 0) coupon.MinConsume = -1;
            if (coupon.Quantity < 0)
            {
                coupon.Quantity = -1;
            }
            else
            {
                coupon.Stock = coupon.Quantity;
            }
            await Service.AddAsync(coupon);
            return Json(new JsonData { Success = true, Msg = "ok", Data = coupon });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReturnCoupon(int id)
        {
            var entity = new SaleReturnCoupon { ID = id, IsDelete = true };
            await Service.UpdateAsync(entity, new[] { nameof(entity.IsDelete) });
            return Json(new JsonData { Success = true, Msg = "ok" });
        }

        [HttpGet]
        public IActionResult ReturnCoupon()
        {
            return PartialView();
        }

    }
}