using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web.Controllers
{
    public class OpenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 订单回调
        /// </summary>
        /// <param name="dada"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public IActionResult DadaCallback([FromBody]DadaCallBack dada, [FromServices]IHostingEnvironment environment)
        {
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            try
            {
                service.UpdateOrderStatus(dada);
            }
            catch (Exception e)
            {
                var filename = Path.Combine(environment.ContentRootPath, "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                System.IO.File.AppendAllText(filename, "\r\n" + Environment.NewLine + e.Message);
            }
            return Ok("更新完成");
        }
    }
}