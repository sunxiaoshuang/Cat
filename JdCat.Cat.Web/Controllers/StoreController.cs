using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using JdCat.Cat.Model;
using Microsoft.Extensions.DependencyInjection;
using JdCat.Cat.Repository.Service;

namespace JdCat.Cat.Web.Controllers
{
    public class StoreController : BaseController<IStoreRepository, TangOrder>
    {
        public StoreController(AppData appData, IStoreRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 堂食订单页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Order()
        {
            return View();
        }
        /// <summary>
        /// 获取堂食订单列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery]PagingQuery paging, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate, [FromQuery]string code)
        {
            var list = await Service.GetOrdersAsync(Business.ID, paging, startDate ?? DateTime.Now, endDate ?? DateTime.Now, code);
            return Json(new {
                rows = list,
                count = paging.RecordCount
            });
        }
        /// <summary>
        /// 获取堂食订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await Service.GetOrderAsync(id);
            return Json(order);
        }
        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderDetail(int id)
        {
            return PartialView(await Service.GetOrderAsync(id));
        }
        /// <summary>
        /// 订单反结账视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ReservePay()
        {
            return PartialView();
        }

        /// <summary>
        /// 获取商户支付方式
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPayments()
        {
            return Json(await Service.GetPaymentsAsync(Business.ID));
        }

        /// <summary>
        /// 更新订单支付方式
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdatePayments(int id, [FromBody]IEnumerable<TangOrderPayment> payments)
        {
            await Service.UpdateOrderPaymentsAsync(id, payments);
            return Json(payments);
        }
        /// <summary>
        /// 获取堂食菜单（简单版）
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetSimpleStoreProducts()
        {
            return Json(await Service.GetSimpleStoreProductsAsync(Business.ID));
        }
        /// <summary>
        /// 反结账时，新增商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<IActionResult> IncreaseOrderProduct([FromBody]TangOrderProduct product)
        {
            product.ProductStatus = TangOrderProductStatus.AddReserve;
            product.Status = EntityStatus.Normal;
            await Service.AddAsync(product);
            var order = await Service.GetAsync<TangOrder>(product.OrderId);
            order.ActualAmount += product.Amount;
            order.Amount += product.Amount;
            order.OriginalAmount += product.Amount;
            await Service.CommitAsync();
            return Json(new {
                order, product
            });
        }
        /// <summary>
        /// 订单商品退货
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RetTangProduct([FromBody]TangOrderProduct product)
        {
            await Service.RetTangProductAsync(product);
            var order = await Service.GetAsync<TangOrder>(product.OrderId);
            order.ActualAmount -= product.Amount;
            order.Amount -= product.Amount;
            order.OriginalAmount -= product.Amount;
            await Service.CommitAsync();
            return Json(order);
        }

    }
}
