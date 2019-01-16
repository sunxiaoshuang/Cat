using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JdCat.Cat.Web.Models;
using JdCat.Cat.Common;
using Microsoft.Extensions.Options;
using JdCat.Cat.Model.Data;
using JdCat.Cat.IRepository;
using Newtonsoft.Json;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Model;

namespace JdCat.Cat.Web.Controllers
{
    public class ChainController : BaseController<IBusinessRepository, Business>
    {
        public ChainController(AppData appData, IBusinessRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 连锁商户主页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (Business.Category == BusinessCategory.Store)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Business = Business;
            return View();
        }

        /// <summary>
        /// 连锁商户门店管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Store()
        {
            return View();
        }
        /// <summary>
        /// 获取所属商户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetStores([FromQuery]PagingQuery query)
        {
            return Json(Service.GetStores(Business.ID, query));
        }

        /// <summary>
        /// 创建门店视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }
        /// <summary>
        /// 绑定门店视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Bind()
        {
            return PartialView();
        }

        /// <summary>
        /// 创建门店
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody]Business business)
        {
            var result = new JsonData();
            var isExist = Service.ExistForCode(business.Code);
            if (isExist)
            {
                result.Msg = "登录帐号已存在！";
                return Json(result);
            }
            business.Password = UtilHelper.MD5Encrypt(business.Password);
            business.RegisterDate = DateTime.Now;
            business.ObjectId = Guid.NewGuid().ToString().ToLower();
            business.ParentId = Business.ID;
            business.Category = BusinessCategory.Store;
            business.AppId = Business.AppId;
            business.Secret = Business.Secret;
            business.TemplateNotifyId = Business.TemplateNotifyId;
            business.AppQrCode = Business.AppQrCode;
            business.StoreId = Service.GetNextStoreNumber();
            business.FeyinMemberCode = Business.FeyinMemberCode;
            business.FeyinApiKey = Business.FeyinApiKey;

            business.BusinessEndTime = Business.BusinessEndTime??"09:00";
            business.BusinessEndTime2 = Business.BusinessEndTime2;
            business.BusinessEndTime3 = Business.BusinessEndTime3;
            business.BusinessStartTime = Business.BusinessStartTime??"06:00";
            business.BusinessStartTime2 = Business.BusinessStartTime2;
            business.BusinessStartTime3 = Business.BusinessStartTime3;
            business.CityCode = Business.CityCode;
            business.CityName = Business.CityName;
            business.Description = Business.Description;
            business.Freight = Business.Freight;
            business.MinAmount = Business.MinAmount;
            business.Range = Business.Range;

            Service.Add(business);
            result.Success = true;
            result.Data = business;
            result.Msg = "新增成功";
            return Json(result);
        }

        /// <summary>
        /// 绑定门店
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Bind(string storeId, string code, string password)
        {
            var result = new JsonData();
            var store = Service.GetBusinessByStoreId(storeId);
            if (store == null)
            {
                result.Msg = "商户编码不存在！";
                return Json(result);
            }
            if (store.Code != code || store.Password != UtilHelper.MD5Encrypt(password))
            {
                result.Msg = "帐号或密码错误！";
                return Json(result);
            }
            Service.BindStore(Business, store);
            result.Data = store;
            result.Success = true;
            result.Msg = "绑定成功";
            return Json(result);
        }

        /// <summary>
        /// 解绑门店
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult UnBind(int id)
        {
            var result = new JsonData();
            var store = Service.Get(id);
            result.Success = Service.UnBindStore(Business, store);
            if (result.Success)
            {
                result.Msg = "解绑成功";
            }
            else
            {
                result.Msg = "解绑失败，门店不存在或已解绑，请刷新后再试";
            }
            return Json(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ResetPwd(int id)
        {
            var password = Service.ResetPwd(id);
            return Content(password);
        }

        /// <summary>
        /// 总部订单
        /// </summary>
        /// <returns></returns>
        public IActionResult Order()
        {
            ViewBag.stores = JsonConvert.SerializeObject(Service.GetStoresOnlyId(Business.ID), AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 获取分店，仅仅id与name
        /// </summary>
        /// <returns></returns>
        public IActionResult GetStoresOnlyId()
        {
            return Json(Service.GetStoresOnlyId(Business.ID));
        }

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrders([FromQuery]PagingQuery query, OrderStatus status, DateTime? startDate, DateTime? endDate, int businessId)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now;
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }
            var list = Service.GetOrders(Business.ID, businessId, status, query, startDate.Value, endDate.Value).Select(a => new { a.Identifier, a.BusinessId, a.OrderCode, a.CreateTime, a.Status, a.Price, a.ReceiverName, a.Phone, a.ReceiverAddress });
            return Json(new
            {
                list,
                rows = query.RecordCount,
                pages = query.PageCount
            });
        }

        /// <summary>
        /// 经营统计
        /// </summary>
        /// <returns></returns>
        public IActionResult Manage()
        {
            ViewBag.stores = JsonConvert.SerializeObject(Service.GetStoresOnlyId(Business.ID), AppData.JsonSetting);
            return View();
        }

        /// <summary>
        /// 营业汇总
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public IActionResult BusinessSummary(DateTime? startDate, DateTime? endDate, int? businessId)
        {
            var report = Service.GetBusinessSummary(Business.ID, businessId??0, startDate??DateTime.Now, endDate??DateTime.Now);
            return Json(report??new List<Model.Report.Report_ChainSummary>());
        }

        /// <summary>
        /// 会员列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UserList()
        {
            return View();
        }

        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserList([FromQuery]PagingQuery query)
        {
            var report = Service.GetUserListByChain(Business.ID, query);
            return Json(new
            {
                rows = query.RecordCount,
                list = report
            });
        }

    }
}
