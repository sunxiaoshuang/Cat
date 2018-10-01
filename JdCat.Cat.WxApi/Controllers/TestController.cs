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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class TestController : BaseController<IUserRepository, User>
    {
        public TestController(IUserRepository service) : base(service)
        {
        }

        [HttpPost("t1")]
        public IActionResult T1([FromBody]dynamic p, [FromServices]IHostingEnvironment env)
        {
            using (var file = System.IO.File.AppendText(Path.Combine(env.ContentRootPath, "t1.txt")))
            {
                file.WriteLine(p.address);
                file.WriteLine("\r\n");
            }
            return Ok("好的");
        }

        /// <summary>
        /// 测试创建订单
        /// </summary>
        /// <returns></returns>
        public IActionResult GetOrder()
        {
            var rep = HttpContext.RequestServices.GetService<IOrderRepository>();
            var business = Service.Set<Business>()
                .Include("Products.Attributes")
                .Include("Products.Formats")
                .Include("Products.Images")
                .OrderBy(a => a.ID).First();
            var user = Service.Set<User>().First();
            var product = business.Products.ElementAt(0);
            var products = new List<OrderProduct> {
                new OrderProduct {
                    Description = "测试订单数据",
                    Product = product,
                    Format =  product.Formats.ElementAt(0),
                    Name = product.Name,
                    Price = product.Formats.ElementAt(0).Price,
                    Quantity = 1,
                    Src = product.Images.ElementAt(0).Name + "." + product.Images.ElementAt(0).ExtensionName
                }
            };
            var order = new Order
            {
                Business = business,
                DeliveryMode = DeliveryMode.Third,
                Freight = business.Freight,
                Lat = 30.499750289775,
                Lng = 114.429076910019,
                PaymentType = PaymentType.OnLine,
                Phone = "17354300837",
                Price = 53,
                Products = products,
                ReceiverAddress = "测试订单数据，地址",
                ReceiverName = "李四",
                Remark = "加椰果",
                Status = OrderStatus.Payed,
                TablewareQuantity = 1,
                Tips = 0,
                Type = OrderType.Food,
                User = user
            };
            rep.Add(order);
            //Task.Run(SendOrderNotify);
            return Ok("ok");
        }

        [HttpGet("TestLog")]
        public IActionResult TestLog()
        {
            Log.Debug("函数式");
            return Json("OK");
        }

        ///// <summary>
        ///// 发送订单通知
        ///// </summary>
        ///// <param name="businessId"></param>
        ///// <returns></returns>
        //private async Task SendOrderNotify(int businessId)
        //{

        //}

    }
}
