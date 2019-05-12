using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseController<IProductRepository, Product>
    {

        public ProductController(IProductRepository service) : base(service)
        {

        }

        /// <summary>
        /// 获取外卖菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("menus/{id}")]
        public IActionResult GetMenus(int id)
        {
            var list = Service.GetTypes(new Business() { ID = id }, ProductStatus.Sale).ToList();
            list.ForEach(a =>
            {
                var tangProduct = a.Products.Where(product => (product.Scope & ActionScope.Takeout) == 0).ToList();
                foreach (var item in tangProduct)
                {
                    a.Products.Remove(item);
                }
                foreach (var item in a.Products)
                {
                    item.Description = item.Description ?? "";
                }
            });
            list.RemoveAll(a => a.Products.Count == 0);
            return Json(list);
        }

        [HttpGet("types/{id}")]
        public IActionResult GetTypes(int id)
        {
            return Json(Service.GetTypes(id));
        }

    }
}
