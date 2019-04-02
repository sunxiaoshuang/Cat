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
    public class CommentController : BaseController<IOrderRepository, Order>
    {
        public CommentController(AppData appData, IOrderRepository service) : base(appData, service)
        {
        }

        public IActionResult Index([FromServices]IBusinessRepository businessRepository)
        {
            ViewBag.pageObj = null;
            if (Business.Category == BusinessCategory.Chain)
            {
                ViewBag.pageObj = JsonConvert.SerializeObject(new
                {
                    isChain = Business.Category == BusinessCategory.Chain,
                    stores = businessRepository.GetStoresOnlyId(Business.ID)
                }, AppData.JsonSetting);
            }
            return View();
        }

        public IActionResult GetComments([FromQuery]PagingQuery paging, [FromQuery]CommentLevel deliveryLevel, [FromQuery]CommentLevel orderLevel, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate, int businessId, [FromServices] IBusinessRepository businessRepository)
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var end = endDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            int[] businessIds;
            if (businessId == 0)
            {
                if (Business.Category == BusinessCategory.Chain)
                {
                    businessIds = businessRepository.GetStoresOnlyId(Business.ID).Select(a => a.Item1).ToArray();
                }
                else
                {
                    businessIds = new[] { Business.ID };
                }
            }
            else
            {
                businessIds = new[] { businessId };
            }
            var list = Service.GetComments(deliveryLevel, orderLevel, start, end, paging, businessIds);
            return Json(new
            {
                rows = list,
                count = paging.RecordCount
            });
        }

        public IActionResult ChangeVisible(int id, [FromQuery]int visible)
        {
            Service.ChangeCommentVisible(id, visible == 1);
            return Ok();
        }

    }
}
