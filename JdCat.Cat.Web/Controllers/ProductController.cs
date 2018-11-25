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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
        public IActionResult Index()
        {
            var types = Service.GetTypes(Business);
            ViewBag.types = types == null ? null : JsonConvert.SerializeObject(types.Select(a => new { a.ID, a.Name, Count = a.Products?.Count() }), AppData.JsonSetting);
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

            return Json(new JsonData
            {
                Success = true,
                Data = types.Select(a => new { a.ID, a.Name, Count = a.Products?.Count() }),
                Msg = "修改成功"
            });
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
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelType(int id)
        {
            
            return null;
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <returns></returns>
        public IActionResult AddProduct(int? id)
        {
            var types = Service.GetProductTypes(Business.ID);
            ViewBag.types = JsonConvert.SerializeObject(types.Select(a => new { a.ID, a.Name, a.Sort }), AppData.JsonSetting);
            ViewBag.attrs = JsonConvert.SerializeObject(Service.GetAttributes().Select(a => new { a.Name, Childs = a.Childs.Select(b => b.Name) }).ToList(), AppData.JsonSetting);
            if (id.HasValue)
            {
                var entity = Service.GetProduct(id.Value);
                if(entity.Feature == ProductFeature.SetMeal)
                {
                    // 如果是套餐，则加载套餐商品
                    entity.Tag1 = Service.GetSetMealProducts(entity.ProductIdSet.Split(',').Select(a => int.Parse(a)).ToArray());
                }
                ViewBag.entity = JsonConvert.SerializeObject(entity, AppData.JsonSetting);
            }
            return View();
        }

        /// <summary>
        /// 选择商品模版
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SelectProduct()
        {
            return View();
        }

        /// <summary>
        /// 套餐可选择的产品数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SelectTypes()
        {
            var types = Service.GetProductWithoutSetMeal(Business.ID)
                .Select(a => new
                {
                    a.ID,
                    a.Name,
                    Products = a.Products.Select(b => new { b.ID, b.Name }).ToList()
                });
            return Json(types);
        }

        /// <summary>
        /// 保存商品
        /// </summary>
        /// <param name="product"></param>
        /// <param name="host"></param>
        /// <returns></returns>
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
            var entity = new Product
            {
                BusinessId = Business.ID,
                Description = product.Description,
                MinBuyQuantity = product.MinBuyQuantity,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                UnitName = product.UnitName,
                Images = product.Images,
                Formats = product.Formats,
                Attributes = product.Attributes,
                ProductIdSet = product.ProductIdSet,
                Feature = product.Feature
            };
            Service.Add(entity);
            result.Success = true;
            result.Msg = "保存成功";
            return Ok(result);
        }
        [HttpGet]
        public IActionResult IsCanUpdate(int id)
        {
            var discount = Service.GetDiscount(id);
            var result = new JsonData {
                Success = discount == null,
            };
            return Json(result);
        }
        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="product"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]ProductModel product, [FromServices]IHostingEnvironment host)
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
                if (product.Images.Count > 0)
                {
                    //Service.DeleteImage(product.Images.First(), AppData.ApiUri, Business.ID);
                    Service.DeleteImage(product.Images.First());
                }
                product.Images = new List<ProductImage> { file };
            }

            // 图片上传成功后，修改商品
            Service.Update(product);
            result.Success = true;
            result.Msg = "修改成功";
            return Ok(result);
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="setting"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProducts([FromQuery]int? typeId, [FromQuery]int pageIndex = 1)
        {
            var list = Service.GetProducts(Business);
            return Json(new { data = list });
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DelProduct(int id)
        {
            var result = new JsonData
            {
                Success = Service.DeleteProduct(id)
            };
            return Json(result);
        }
        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Up(int id)
        {
            Service.Up(id);
            Service.Commit();
            return Ok("上架成功");
        }
        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Down(int id)
        {
            Service.Down(id);
            Service.Commit();
            return Ok("下架成功");
        }

        [HttpPut]
        public IActionResult BatchUp([FromBody]IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                Service.Up(id);
            }
            var result = new JsonData { Success = Service.Commit() > 0 };
            result.Msg = result.Success ? "批量上架成功" : "批量上架失败";
            return Json(result);
        }
        [HttpPut]
        public IActionResult BatchDown([FromBody]IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                Service.Down(id);
            }
            var result = new JsonData { Success = Service.Commit() > 0 };
            result.Msg = result.Success ? "批量下架成功" : "批量下架失败";
            return Json(result);
        }

        [HttpPut]
        public IActionResult BatchRemove([FromBody] IEnumerable<int> ids)
        {
            var result = new JsonData { Success = Service.DeleteProduct(ids.ToArray()) };
            result.Msg = result.Success ? "批量删除成功" : "没有找到删除的商品，可以已经被删除了";
            return Json(result);
        }

    }
}