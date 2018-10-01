using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Web.App_Code;
using log4net;
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
        /// 达达订单回调
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
        /// <summary>
        /// 点我达订单回调
        /// </summary>
        /// <param name="dada"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public async Task<IActionResult> DWDCallback([FromBody]DWD_Callback dwd, [FromServices]DwdHelper helper)
        {
            var orderCode = dwd.order_original_id.Split('_')[0];
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            try
            {
                var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
                log.Debug("点我达回调：" + JsonConvert.SerializeObject(dwd));
                // 每次回调时，均重新读取一次订单配送费用，并保存到数据库
                var priceResult = await helper.GetOrderPrice(orderCode);
                double cost = 0;
                if (priceResult.success)
                {
                    cost = priceResult.result.receivable_price / 100;
                }
                service.UpdateOrderByDwd(dwd, cost);
            }
            catch (Exception e)
            {
                var log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(OpenController));
                log.Info("点我达回调错误：" + e.Message);
            }


            return Json(new { success = true });
        }

    }
}