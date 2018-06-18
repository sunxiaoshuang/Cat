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
    public class Test2Controller : BaseController<IBusinessRepository, Business>
    {
        public Test2Controller(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }

        public IActionResult CreateOrder([FromBody]Order order)
        {
            var service = HttpContext.RequestServices.GetService<IOrderRepository>();
            order.Freight = Business.Freight;
            order.Lat = 30.499750289775;
            order.Lng = 114.429076910019;
            order.TablewareQuantity = 1;
            order.Status = Model.Enum.OrderStatus.Payed;
            order.CityCode = Business.CityCode;
            order.UserId = service.Set<User>().FirstOrDefault().ID;
            order.BusinessId = Business.ID;
            var result = new JsonData { Success = service.Add(order) > 0};
            return Ok(result);
        }
    }
}