using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class DianwodaController : BaseController<IDwdRepository, DWD_Business>
    {
        public DianwodaController(AppData appData, IDwdRepository service) : base(appData, service)
        {
        }

        public async Task<IActionResult> Index([FromServices]DwdHelper helper)
        {
            var dwd = Service.Get(a => a.BusinessId == Business.ID);
            long balance = 0;
            if(dwd != null)
            {
                var back = await helper.GetBalance(dwd.external_shopid);
                balance = back.result.balance;
            }
            else
            {
                dwd = new DWD_Business();
            }
            
            ViewBag.dwd = JsonConvert.SerializeObject(dwd, AppData.JsonSetting);
            ViewBag.balance = balance;

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var dwd = new DWD_Business
            {
                addr = Business.Address,
                external_shopid = Guid.NewGuid().ToString(),
                lat = long.Parse(Business.Lat.ToString().Replace(".", "")),
                lng = long.Parse(Business.Lng.ToString().Replace(".", "")),
                shop_title = Business.Name,
                mobile = Business.Mobile,
                BusinessId = Business.ID
            };
            ViewBag.dwd = JsonConvert.SerializeObject(dwd, AppData.JsonSetting);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]DWD_Business dwd, [FromServices]DwdHelper helper)
        {
            var result = new JsonData();
            var back = await helper.CreateShop(dwd);
            if (!back.success)
            {
                result.Msg = back.message;
                return Json(result);
            }
            result.Success = Service.CreateShop(dwd);
            result.Msg = "创建成功";
            result.Data = dwd;
            return Json(result);
        }


    }
}
