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
        public async Task<IActionResult> Init([FromServices]IBusinessRepository servcie, [FromServices]AppData appData)
        {
            var businesses = await servcie.GetAllAsync<Business>();
            foreach (var business in businesses)
            {
                business.WeChatAppId = appData.WeChatAppId;
                business.WeChatSecret = appData.WeChatSecret;
                business.NewOrderTemplateId = appData.EventMessageTemplateId;
                business.RefundTemplateId = appData.Msg_Refund;
                business.PayServerAppId = appData.ServerAppId;
                business.PayServerKey = appData.ServerKey;
                business.PayServerMchId = appData.ServerMchId;
                business.CertFile = appData.CertFile;
            }
            await servcie.CommitAsync();

            return Ok("初始化成功");
        }

    }
}
