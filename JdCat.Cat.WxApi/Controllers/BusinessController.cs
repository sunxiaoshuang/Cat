using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using JdCat.Cat.Common.Models;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class BusinessController : BaseController<IBusinessRepository, Business>
    {
        public BusinessController(IBusinessRepository service) : base(service)
        {

        }
        
        [HttpGet("{id}")]
        public IActionResult GetBusiness(int id)
        {
            return Json(Service.Get(a => a.ID == id));
        }

        [HttpGet("fullreduce/{id}")]
        public IActionResult GetFullReduce(int id)
        {
            var list = Service.GetFullReduce(new Business { ID = id }, false).ToList();
            var valid = list.Where(a => a.IsActiveValid());
            return Json(valid);
        }

        [HttpGet("coupon/{id}")]
        public IActionResult GetCoupon(int id)
        {
            return Json(Service.GetCouponValid(new Business { ID = id }));
        }

    }
}
