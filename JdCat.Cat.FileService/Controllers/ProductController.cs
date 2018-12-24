using System;
using System.Collections.Generic;
using Fs = System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.FileService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using JdCat.Cat.Common.Models;

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
                if (Fs.File.Exists(item))
                {
                    Fs.File.Delete(item);
                }
            }
            return Ok("删除成功");
        }

        [HttpPost("Copy")]
        public IActionResult Copy([FromBody]CopyProduct copyData, [FromServices]IHostingEnvironment environment)
        {
            var productPath = $"{environment.WebRootPath}/{ProductImage._defaultPath}";
            var parentPath = $"{productPath}/{copyData.ChainId}";
            var formatArr = new[] { "400x300", "200x150", "100x75" };
            // 首先获取所有需要复制的文件路径
            var imageDic = new Dictionary<string, List<string>>();
            foreach (var format in formatArr)
            {
                var list = new List<string>();
                foreach (var name in copyData.ImageNames)
                {
                    var filePath = $"{parentPath}/{format}/{name}";
                    if (!Fs.File.Exists(filePath)) continue;
                    list.Add(filePath);
                }
                imageDic.Add(format, list);
            }

            // 然后给每个商户复制
            foreach (var id in copyData.StoreIds)
            {
                var dir = $"{productPath}/{id}";
                if (!Fs.Directory.Exists(dir)) { Fs.Directory.CreateDirectory(dir); }
                foreach (var format in formatArr)
                {
                    var formatDir = $"{dir}/{format}";
                    if (!Fs.Directory.Exists(formatDir)) Fs.Directory.CreateDirectory(formatDir);
                    foreach (var filepath in imageDic[format])
                    {
                        Fs.File.Copy(filepath, $"{formatDir}/{Fs.Path.GetFileName(filepath)}");
                    }
                }
            }
            return Ok("复制成功");
        }
    }
}