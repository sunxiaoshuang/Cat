using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.WxApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class AppController : BaseController<IBusinessRepository, Business>
    {
        public AppController(IBusinessRepository service) : base(service)
        {
        }

        [HttpGet("getLocations/{location}")]
        public async Task<IActionResult> GetLocations(string location)
        {
            var result = await WxHelper.GetAddressAsync(location);
            return Ok(result);
        }

        [HttpGet("getNearestStore/{id}")]
        public IActionResult GetNearestStore(int id, [FromQuery]string location)
        {
            var position = location.Split(',').Select(a => Convert.ToDouble(a)).ToList();
            var business = Service.GetNearestStore(id, position[0], position[1]);
            var result = new JsonData { Success = true, Data = business, Msg = "ok" };
            return Json(result);
        }

        [HttpGet("getNearbyStore/{id}")]
        public IActionResult GetNearbyStore(int id, [FromQuery]string city, [FromQuery]string key, [FromQuery]string location)
        {
            var position = location.Split(',').Select(a => Convert.ToDouble(a)).ToList();
            var list = Service.GetNearbyStore(id, city, position[0], position[1], key);
            if(list == null)
            {
                list = new List<Business>();
            }
            var point = new Tuple<double, double>(position[0], position[1]);
            list.ForEach(a => a.Distance = new Tuple<double, double>(a.Lat, a.Lng).GetDistance(point));
            return Json(list);
        }

        private static string cityJson;
        [HttpGet("getCity")]
        public IActionResult GetCity([FromServices]IHostingEnvironment env)
        {
            if (string.IsNullOrEmpty(cityJson))
            {
                cityJson = System.IO.File.ReadAllText(Path.Combine(env.WebRootPath, "assert", "city.json"), Encoding.UTF8);
            }
            Response.ContentType = "application/json; charset=utf-8";
            return Content(cityJson);
        }
    }
}
