using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class ProductController : BaseController<IProductRepository, Product>
    {
        public ProductController(AppData appData, IProductRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 商品管理页面
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IActionResult Index([FromServices] JsonSerializerSettings settings)
        {
            var types = Service.GetTypes(Business);
            ViewBag.types = types == null ? null : JsonConvert.SerializeObject(types.Select(a => new { a.ID, a.Name, Count = a.Products?.Count() }), settings);
            return View();
        }

        /// <summary>
        /// 添加商品类别页面
        /// </summary>
        /// <returns></returns>
        public IActionResult AddType()
        {
            var types = Service.GetTypes(Business);
            return PartialView(types);
        }

        /// <summary>
        /// 修改商品类别
        /// </summary>
        /// <param name="add">新增的类别</param>
        /// <param name="edit">需要修改的类别</param>
        /// <param name="remove">删除的类别</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateTypes(IEnumerable<ProductType> add, IEnumerable<ProductType> edit, IEnumerable<int> remove)
        {
            if (add.Count() > 0)
            {
                foreach (var item in add)
                {
                    item.BusinessId = Business.ID;
                }
                Service.AddTypes(add);
            }
            if (edit.Count() > 0)
            {
                Service.EditTypes(edit);
            }
            if (remove.Count() > 0)
            {
                Service.RemoveTypes(remove);
            }
            Service.Commit();

            var types = Service.GetTypes(Business);

            return Json(new JsonData { Success = true,
                Data = types.Select(a => new { a.ID, a.Name, Count = a.Products?.Count() }),
                Msg = "修改成功" });
        }

        /// <summary>
        /// 判断指定的类别下是否存在商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ExistProduct(int id)
        {
            var result = new JsonData();
            result.Data = Service.ExistProduct(id);
            result.Success = true;
            return Json(result);
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <returns></returns>
        public IActionResult AddProduct([FromServices] JsonSerializerSettings settings)
        {
            var types = Service.GetTypes(Business);
            ViewBag.types = types == null ? null : JsonConvert.SerializeObject(types.Select(a => new { a.ID, a.Name }), settings);
            return View();
        }

        [HttpPost]
        public IActionResult Save([FromBody]Product product)
        {
            product.BusinessId = Business.ID;
            Service.Add(product);
            return Json(product);
        }

    }
}