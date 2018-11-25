using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using JdCat.Cat.Model;
using Microsoft.Extensions.DependencyInjection;
using JdCat.Cat.Repository.Service;

namespace JdCat.Cat.Web.Controllers
{
    public class DianwodaController : BaseController<IDwdRepository, DWDStore>
    {
        /// <summary>
        /// 点我达充值编码
        /// </summary>
        public string DwdBizNo
        {
            get => HttpContext.Session.Get<string>("Dwd_Recharge_Flow");
            set => HttpContext.Session.Set("Dwd_Recharge_Flow", value);
        }
        public DianwodaController(AppData appData, IDwdRepository service) : base(appData, service)
        {
        }

        /// <summary>
        /// 点我达首页
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public IActionResult Index([FromServices]DwdHelper helper)
        {
            if (Business.DWDStore == null)
            {
                var dwd = Service.Get(a => a.BusinessId == Business.ID);
                if (dwd != null)
                {
                    Business.DWDStore = dwd;
                    // 保存点我达商户状态
                    SetStore(dwd);
                }
            }
            ViewBag.exist = Business.DWDStore != null;
            return View();
        }

        /// <summary>
        /// 点我达商户信息页面
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> Info([FromServices]DwdHelper helper)
        {
            long balance = 0;
            DWDStore dwd = Business.DWDStore;
            if (dwd != null)
            {
                var back = await helper.GetBalanceAsync(dwd.external_shopid);
                balance = back.result.balance;
            }
            else
            {
                dwd = new DWDStore();
            }

            ViewBag.dwd = JsonConvert.SerializeObject(dwd, AppData.JsonSetting);
            ViewBag.balance = balance;

            return View();
        }

        /// <summary>
        /// 点我达交易明细页面
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public IActionResult Detail()
        {
            return View();
        }

        /// <summary>
        /// 点我达交易列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailList([FromQuery]int pageIndex, [FromQuery]DateTime startDate, [FromQuery] DateTime endDate, [FromServices]DwdHelper helper)
        {
            var back = await helper.GetDetailAsync(Business.DWDStore, startDate, endDate, pageIndex);
            return Json(back);
        }

        public IActionResult OrderList([FromBody]PagingQuery query, [FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            var rep = HttpContext.RequestServices.GetService<IOrderRepository>();
            var list = rep.GetOrder(Business, null, query, null, null, expression: order => order.LogisticsType == LogisticsType.Dianwoda && order.CreateTime >= startDate && order.CreateTime < endDate.AddDays(1)).ToList();
            return Json(list);
        }


        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> OrderDetail([FromQuery]string orderCode, [FromServices]DwdHelper helper)
        {
            var back = await helper.GetOrderDetailAsync(orderCode);
            var price = await helper.GetOrderPriceAsync(orderCode);
            ViewBag.typeName = price.result.price_type == 1 ? "预估费用" : "最终费用";
            ViewBag.price = "￥" + (price.result.receivable_price / 100);
            return View(back.result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var dwd = new DWDStore
            {
                addr = Business.Address,
                external_shopid = Guid.NewGuid().ToString(),
                lat = Business.Lat,
                lng = Business.Lng,
                shop_title = Business.Name,
                mobile = Business.Mobile,
                BusinessId = Business.ID
            };
            ViewBag.dwd = JsonConvert.SerializeObject(dwd, AppData.JsonSetting);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]DWDStore dwd, [FromServices]DwdHelper helper)
        {
            var result = new JsonData();
            var back = await helper.CreateShopAsync(dwd);
            if (!back.success)
            {
                result.Msg = back.message;
                return Json(result);
            }
            result.Success = Service.CreateShop(dwd);
            result.Msg = "创建成功";
            result.Data = dwd;
            // 保存点我达商户状态
            SetStore(dwd);
            return Json(result);
        }

        //public async Task<IActionResult> Recharge([FromServices]DwdHelper helper)
        //{
        //    var dwd = Business.DWDStore;
        //    var model = new DWD_Recharge { Amount = 0.01, Code = "12345688855", DWD_Business = dwd, Mode = Common.Dianwoda.DwdRechargeType.Alipay };

        //    var back = await helper.Recharge(model);
        //    var arr = UtilHelper.CreateCodeEwm(back.result.pay_content);
        //    return File(arr, "application/x-png");

        //}

        public IActionResult RechargePage([FromQuery]string type, [FromQuery]double amount)
        {
            ViewBag.type = type;
            ViewBag.amount = amount;
            return View();
        }

        /// <summary>
        /// 支付宝付款
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Alipay([FromQuery]double amount, [FromServices]DwdHelper helper)
        {
            var dwd = Service.Get(a => a.BusinessId == Business.ID);
            var recharge = new DWD_Recharge { Amount = amount, DWD_Business = dwd, Mode = DwdRechargeType.Alipay, Code = $"Dwd{DateTime.Now:yyyyMMddHHmmss}{UtilHelper.RandNum()}" };
            var back = await helper.RechargeAsync(recharge);
            if (back.success)
            {
                ViewBag.form = back.result.pay_content;
                recharge.DwdCode = back.result.biz_no;
                Service.AddRecharge(recharge);
                DwdBizNo = recharge.DwdCode;
                return View();
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 微信付款
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Wechat([FromQuery]double amount, [FromServices]DwdHelper helper)
        {
            var dwd = Service.Get(a => a.BusinessId == Business.ID);
            var recharge = new DWD_Recharge { Amount = amount, DWD_Business = dwd, Mode = DwdRechargeType.Wechat, Code = $"Dwd{DateTime.Now:yyyyMMddHHmmss}{UtilHelper.RandNum()}" };
            var back = await helper.RechargeAsync(recharge);
            if (back.success)
            {
                ViewBag.url = back.result.pay_content;
                recharge.DwdCode = back.result.biz_no;
                Service.AddRecharge(recharge);
                DwdBizNo = recharge.DwdCode;
                ViewBag.model = JsonConvert.SerializeObject(recharge, AppData.JsonSetting);
                return View();
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 微信充值二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IActionResult WechatCode([FromQuery]string url)
        {
            var arr = UtilHelper.CreateCodeEwm(url);
            return File(arr, "application/x-png");
        }

        /// <summary>
        /// 查询充值结果
        /// </summary>
        /// <param name="biz_no"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public async Task<IActionResult> RechargeResult([FromServices]DwdHelper helper)
        {
            var dwd = Business.DWDStore;
            if (string.IsNullOrEmpty(DwdBizNo))
            {
                return Json(new { success = false });
            }
            var back = await helper.RechargeResultAsync(dwd, DwdBizNo);
            if(back.success && back.result.rechange_succ)
            {
                Service.RechargeSuccess(DwdBizNo);
            }
            return Json(back);
        }

        /// <summary>
        /// 充值完成页面
        /// </summary>
        /// <returns></returns>
        public IActionResult RechargeFinish()
        {
            return View();
        }

        //public async Task<IActionResult> DealDetail()
        //{

        //}

        /// <summary>
        /// 将点我达商户保存到Session
        /// </summary>
        /// <param name="shop"></param>
        private void SetStore(DWDStore shop)
        {
            Business.DWDStore = shop;
            HttpContext.Session.Set(AppData.Session, Business);
        }

    }
}
