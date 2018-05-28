using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
//    [EnableCors("AllowDomain")]
    public class ProductController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get([FromServices]IHostingEnvironment env)
        {
            return new string[] { "香蕉", "苹果" };
        }
        
    }
}
