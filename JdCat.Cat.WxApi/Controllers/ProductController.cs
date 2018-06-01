using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
//    [EnableCors("AllowDomain")]
    public class ProductController : BaseController<IProductRepository, Product>
    {

        public ProductController(IProductRepository service):base(service)
        {

        }
        // GET api/values
        [HttpGet]
        public string Get([FromServices]IHostingEnvironment env)
        {
            return "api test success";
        }

        [HttpGet]
        [Route("menus")]
        public string GetMenus(int businessId)
        {
            var business = new Business() { ID = businessId };
            var types = Service.GetTypes(business);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(types, settings);
        }
    }
}
