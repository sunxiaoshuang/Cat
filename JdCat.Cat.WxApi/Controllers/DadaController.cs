using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.WxApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class DadaController : Controller
    {
        public IActionResult Index([FromBody]DadaCallbackModel model, [FromServices]IHostingEnvironment envi)
        {
            using (var sw = System.IO.File.AppendText(Path.Combine(envi.WebRootPath, "dada.txt")))
            {
                sw.WriteLine(JsonConvert.SerializeObject(model));
            }
            return Ok("好的");
        }
    }
}