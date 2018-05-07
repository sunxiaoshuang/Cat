using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.Models;
using Microsoft.AspNetCore.Hosting;
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
        /// 商品管理页面添加商品类别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddType([FromBody]ProductType entity)
        {
            entity.BusinessId = Business.ID;
            Service.AddTypes(new[] { entity });
            Service.Commit();
            return Json(new { entity.ID, entity.Name, entity.Sort });
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
        public IActionResult AddProduct([FromServices]JsonSerializerSettings settings)
        {
            var types = Service.GetTypes(Business);
            ViewBag.types = types == null ? null : JsonConvert.SerializeObject(types.Select(a => new { a.ID, a.Name, a.Sort }), settings);
            ViewBag.attrs = JsonConvert.SerializeObject(Service.GetAttributes().Select(a => new {a.Name, Childs = a.Childs.Select(b => b.Name)}).ToList(), settings);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody]ProductModel product, [FromServices]IHostingEnvironment host)
        {
            var result = new JsonData();
            if (!string.IsNullOrEmpty(product.Img400))
            {
                var file = new ProductImage
                {
                    CreateTime = DateTime.Now,
                    Name = Guid.NewGuid().ToString().ToLower(),
                    Type = ImageType.Product,
                    Length = Convert.FromBase64String(product.Img400.Replace("data:image/jpeg;base64,", "")).Length
                };
                // 上传图片
                var msg = await Service.UploadImageAsync(AppData.ApiUri + "/Product", Business.ID, file.Name + "." + file.ExtensionName, product.Img400, product.Img200, product.Img100);
                if (msg != "ok")
                {
                    result.Msg = msg;
                    return Json(result);
                }
                product.Images = new List<ProductImage> { file };
            }

            // 图片上传成功后，保存商品
            var entity = new Product {
                BusinessId = Business.ID,
                Description = product.Description,
                MinBuyQuantity = product.MinBuyQuantity,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                UnitName = product.UnitName,
                Images = product.Images,
                Formats = product.Formats,
                Attributes = product.Attributes
            };
            Service.Add(entity);
            result.Success = true;
            result.Msg = "保存成功";
            return Ok(result);
        }


    }
}