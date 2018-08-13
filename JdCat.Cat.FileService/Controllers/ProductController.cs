using System;
using System.Collections.Generic;
using Fs = System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.FileService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.FileService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : BaseController
    {
        [HttpPost]
        public IActionResult Post([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            image.Save(hosting);
            return Ok("保存成功");
        }
        [HttpDelete]
        public IActionResult Delete(string name, int businessId, [FromServices]IHostingEnvironment environment)
        {
            var path = $"{environment.WebRootPath}/{ProductImage._defaultPath}/{businessId}";
            var paths = new[] { $"{path}/400x300/{name}", $"{path}/200x150/{name}", $"{path}/100x75/{name}" };
            foreach (var item in paths)
            {
                if(Fs.File.Exists(item))
                {
                    Fs.File.Delete(item);
                }
            }
            return Ok("删除成功");
        }
    }
}