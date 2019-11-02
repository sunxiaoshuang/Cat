using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    /// <summary>
    /// 通用控制器
    /// </summary>
    public class UtilController : BaseController<IUtilRepository, Business>
    {
        public UtilController(AppData appData, IUtilRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 当系统的数据需要更新时，执行该方法
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Init([FromServices]AppData appData)
        {


            return Ok("同步订单活动完成");
        }

    }
}
