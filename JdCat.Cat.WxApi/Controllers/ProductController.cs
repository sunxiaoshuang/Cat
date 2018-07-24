using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
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

        public ProductController(IProductRepository service):base(service)
        {

        }

        [HttpGet("menus/{id}")]
        public IActionResult GetMenus(int id)
        {
            return Json(Service.GetTypes(new Business() { ID = id }, Model.Enum.ProductStatus.Sale));
        }

    }
}
