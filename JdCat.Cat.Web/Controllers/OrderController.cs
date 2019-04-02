using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using JdCat.Cat.Repository.Model;
using JdCat.Cat.Repository.Service;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

namespace JdCat.Cat.Web.Controllers
{
    public class OrderController : BaseController<IOrderRepository, Order>
    {
        public OrderController(AppData appData, IOrderRepository service) : base(appData, service)
        {

        }

        /// <summary>
        /// 订单列表页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index([FromQuery]string code, [FromServices]List<DadaCancelReason> reasonList)
        {
            ViewBag.reasonList = JsonConvert.SerializeObject(reasonList, AppData.JsonSetting);
            ViewBag.deviceList = JsonConvert.SerializeObject(Service.GetPrinters(Business), AppData.JsonSetting);
            ViewBag.code = code + "";
            return View();
        }

        /// <summary>
        /// 获取订单记录
        /// </summary>
        /// <param name="status"></param>
        /// <param name="query"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetOrder([FromQuery]int status, [FromQuery]string code, [FromQuery]string phone, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            var state = status == 0 ? null : (OrderStatus?)status;
            IEnumerable<Order> list = null;
            if (startDate != null && endDate != null)
            {
                list = Service.GetOrders(Business, state, query, code, phone, expression: a => a.CreateTime >= startDate && a.CreateTime < endDate.Value.AddDays(1));
            }
            else
            {
                var nowDate = DateTime.Now.ToString("yyyy-MM-dd");
                list = Service.GetOrders(Business, state, query, code, phone, expression: a => a.CreateTime.Value.ToString("yyyy-MM-dd") == nowDate);
            }
            result.Data = new
            {
                list,
                rows = query.RecordCount
            };
            result.Success = true;
            return Json(result);
        }

        /// <summary>
        /// 获取订单记录（仅订单基本内容）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="code"></param>
        /// <param name="phone"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IActionResult GetOrders([FromQuery]OrderStatus? status, [FromQuery]string code, [FromQuery]string phone, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate, [FromBody]PagingQuery query)
        {
            var result = new JsonData();
            IEnumerable<Order> list = null;
            var state = status == 0 ? null : (OrderStatus?)status;
            if (startDate != null && endDate != null)
            {
                list = Service.GetOrders(Business, state, query, code, phone, expression: a => a.CreateTime >= startDate && a.CreateTime < endDate.Value.AddDays(1));
            }
            else
            {
                var nowDate = DateTime.Now.ToString("yyyy-MM-dd");
                list = Service.GetOrders(Business, state, query, code, phone, expression: a => a.CreateTime.Value.ToString("yyyy-MM-dd") == nowDate);
            }
            result.Data = new
            {
                list,
                rows = query.RecordCount
            };
            result.Success = true;
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetOrderDetail(int id)
        {
            return Json(Service.GetOrderForDetail(id));
        }

        /// <summary>
        /// 获取订单详情视图
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OrderDetailView(int id)
        {
            return PartialView(Service.GetOrderOnlyProduct(id));
        }

        /// <summary>
        /// 接单
        /// </summary>
        public IActionResult Receive(int id)
        {
            var result = new JsonData
            {
                Success = Service.Receive(id)
            };
            result.Msg = result.Success ? "接单成功" : "接单异常或者已经接单";
            return Json(result);
        }

        /// <summary>
        /// 拒单
        /// </summary>
        public IActionResult Reject(int id, [FromQuery]string msg, [FromServices]IHostingEnvironment _env)
        {
            var certPath = Path.Combine(_env.ContentRootPath, "Asserts", AppData.CertFile);
            var result = Service.Reject(id, msg, new X509Certificate2(certPath, AppData.ServerMchId));
            return Json(result);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        public IActionResult CancelOrder(int id, [FromQuery]string reason, [FromServices]IHostingEnvironment _env)
        {
            var certPath = Path.Combine(_env.ContentRootPath, "Asserts", AppData.CertFile);
            var result = Service.Cancel(id, reason, new X509Certificate2(certPath, AppData.ServerMchId));
            return Json(result);
        }

        /// <summary>
        /// 订单配送
        /// </summary>
        /// <param name="id"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public async Task<IActionResult> Send(int id, [FromQuery]int type, [FromQuery]LogisticsType logisticsType)
        {
            //JsonData result;
            //if (type == 0)
            //{
            //    var order = Service.Get(id);
            //    order.LogisticsType = logisticsType;
            //    result = await Service.Invoice(order);
            //    Service.Commit();
            //}
            //else
            //{
            //    result = new JsonData();

            //    // 自己配送
            //    result.Success = Service.SendOrderSelf(id);
            //    result.Msg = result.Success ? "操作成功" : "接单异常或者已经接单";
            //    result.Data = new
            //    {
            //        Mode = DeliveryMode.Own,
            //        Status = OrderStatus.Distribution,
            //    };
            //    return Json(result);
            //}
            var order = Service.GetOrderIncludeProduct(id);
            order.LogisticsType = logisticsType;
            var result = await Service.Invoice(order);
            Service.Commit();
            return Json(result);
        }

        //private async Task<JsonData> DadaHandler(Order order)
        //{
        //    var result = new JsonData();
        //    var helper = HttpContext.RequestServices.GetService<DadaHelper>();
        //    var back = await helper.SendOrderAsync(order, Business);
        //    // 发送订单未成功
        //    if (!back.IsSuccess())
        //    {
        //        result.Msg = back.msg;
        //        order.ErrorReason = back.msg;
        //        Service.Commit();
        //        return result;
        //    }
        //    result.Success = Service.SendSuccess(order, back);
        //    result.Msg = result.Success ? "配送成功" : "接单异常或者已经接单";
        //    result.Data = new
        //    {
        //        Mode = DeliveryMode.Third,
        //        Logistics = LogisticsType.Dada,
        //        order.Status
        //    };
        //    return result;
        //}
        //private async Task<JsonData> DwdHandler(Order order)
        //{
        //    var result = new JsonData();
        //    var helper = HttpContext.RequestServices.GetService<DwdHelper>();
        //    var shop = GetDwdShop();
        //    if (shop == null)
        //    {
        //        result.Msg = "尚未创建点我达商户，请进入[第三方外卖管理->点我达设置]完成初始化操作";
        //        return result;
        //    }
        //    var back = await helper.SendOrderAsync(order, Business);
        //    // 发送订单未成功
        //    if (!back.success)
        //    {
        //        if(back.message == "服务不可用")
        //        {
        //            back.message = "账户余额不足，请充值，充值方法：第三方外卖管理->点我达设置->充值";
        //        }
        //        result.Msg = back.message;
        //        order.ErrorReason = back.message;
        //        Service.Commit();
        //        return result;
        //    }
        //    // 订单发送成功后，调用接口获取当前订单的配送费，并写入订单信息中
        //    var priceResult = await helper.GetOrderPrice(helper.GetOrderCode(order));
        //    if (priceResult.success)
        //    {
        //        order.CallbackCost = priceResult.result.receivable_price / 100;
        //    }
        //    result.Success = Service.SendDwdSuccess(order, back);
        //    result.Msg = result.Success ? "配送成功" : "接单异常或者已经接单";
        //    result.Data = new
        //    {
        //        Mode = order.DeliveryMode,
        //        Logistics = order.LogisticsType,
        //        order.Status,
        //        flow = order.DistributionFlow
        //    };
        //    return result;
        //}

        /// <summary>
        /// 配送完成，自己配送的订单，需要在管理页面点击完成按钮
        /// </summary>
        /// <returns></returns>
        public IActionResult Achieve(int id)
        {
            var result = new JsonData
            {
                Success = Service.Achieve(id)
            };
            result.Msg = result.Success ? "配送完成" : "订单不存在";
            result.Data = OrderStatus.Achieve;
            return Json(result);
        }

        /// <summary>
        /// 取消配送
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flagId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<IActionResult> Cancel(int id, [FromQuery]int flagId, [FromQuery]string reason)
        {
            JsonData result;
            var order = Service.Get(a => a.ID == id);
            if (order.DeliveryMode == DeliveryMode.Third)
            {
                return Json(await CancelDistribution(order));
            }

            result = new JsonData();
            order.Status = OrderStatus.Receipted;
            result.Success = Service.Commit() > 0;
            result.Data = OrderStatus.Receipted;
            result.Msg = "配送取消成功";
            return Json(result);
        }

        /// <summary>
        /// 打印小票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="device_no"></param>
        /// <returns></returns>
        public async Task<IActionResult> Print(int id, [FromQuery]int device_id)
        {
            var order = Service.GetOrderIncludeProduct(id);
            var device = Service.GetPrinter(device_id);
            var result = await Print(order, device);
            return Json(result);
        }

        /// <summary>
        /// 添加小费
        /// </summary>
        /// <param name="id">订单id</param>
        /// <param name="tip">小费</param>
        /// <param name="code">订单编号</param>
        /// <param name="distributionFlow">配送流水</param>
        /// <returns></returns>
        public async Task<IActionResult> AddTip(int id, double tip, string code, int distributionFlow)
        {
            // 目前仅支持点我达增加小费
            var orderCode = code + "_" + distributionFlow;
            var helper = HttpContext.RequestServices.GetService<DwdHelper>();
            var result = await helper.AddTip(orderCode, (int)(tip * 100));
            return Json(result);
        }

        /// <summary>
        /// 历史订单
        /// </summary>
        /// <returns></returns>
        public IActionResult History()
        {

            return View();
        }

        /// <summary>
        /// ws消息处理
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> MessageHandler([FromQuery]string code)
        //{
        //    // Data记录语音提示方式，Msg记录提示语
        //    var result = new JsonData();
        //    if (!Business.IsAutoReceipt)
        //    {
        //        result.Success = true;
        //        result.Data = 1;
        //        result.Msg = "商家不自动接单";
        //        return Json(result);
        //    }

        //    var order = Service.GetOrderByCode(code);
        //    switch (Business.ServiceProvider)
        //    {
        //        case LogisticsType.None:
        //            order.Status = OrderStatus.Receipted;
        //            result.Data = 2;
        //            result.Success = Service.Commit() > 0;
        //            break;
        //        case LogisticsType.Self:
        //            // 自己配送
        //            result.Success = Service.SendOrderSelf(order.ID, order);
        //            result.Data = 2;
        //            result.Msg = "自动接单成功，配送方式：自己配送";
        //            break;
        //        case LogisticsType.Dada:
        //            // 达达配送
        //            result = await DadaHandler(order);
        //            result.Data = result.Success ? 2 : 3;
        //            break;
        //        case LogisticsType.Dianwoda:
        //            // 点我达配送
        //            result = await DwdHandler(order);
        //            result.Data = result.Success ? 2 : 3;
        //            break;
        //        default:
        //            break;
        //    }
        //    // 打印小票
        //    //var json = await Print(order, Business.DefaultPrinterDevice);
        //    //if (!json.Success)
        //    //{
        //    //    result.Success = false;
        //    //    result.Msg += "|" + json.Msg;
        //    //}

        //    return Json(result);
        //}

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="device_no"></param>
        /// <returns></returns>
        private async Task<JsonData> Print(Order order, FeyinDevice device)
        {
            var result = new JsonData();

            var content = await Service.Print(order, device, Business);
            if(device.Type == PrinterType.Feyin)
            {
                var ret = JsonConvert.DeserializeObject<FeyinModel>(content);
                result.Success = ret.ErrCode == null || ret.ErrCode == 0;
                result.Msg = ret.ErrMsg;
            }
            else if(device.Type == PrinterType.Yilianyue)
            {
                var ret = JsonConvert.DeserializeObject<YlyReturn>(content);
                result.Success = ret.state == "1";
                if (!result.Success)
                {
                    result.Msg = ret.state == "4" ? "打印签名错误" : "打印参数错误";
                }
            }
            else if(device.Type == PrinterType.Feie)
            {
                var ret = JsonConvert.DeserializeObject<FeieReturn>(content);
                result.Success = ret.ret == 0;
                if (!result.Success)
                {
                    result.Msg = ret.msg;
                }
            }
            else if (device.Type == PrinterType.Waimaiguanjia)
            {
                var ret = JsonConvert.DeserializeObject<WmgjReturn<object>>(content);
                result.Success = ret.errno == 0;
                if (!result.Success)
                {
                    result.Msg = ret.msg;
                }
            }

            if (!result.Success)
            {
                // 打印失败
                return result;
            }
            result.Msg = "正在打印小票，请稍等";
            return result;
        }
        private async Task<JsonData> CancelDistribution(Order order)
        {
            switch (order.LogisticsType)
            {
                case LogisticsType.Dada:
                    return await DadaCancel(order);
                case LogisticsType.Dianwoda:
                    return await DwdCancel(order);
                case LogisticsType.Yichengfeike:
                    return await YcfkCancel(order);
                default:
                    break;
            }
            return null;
        }
        private async Task<JsonData> DadaCancel(Order order)
        {
            var result = new JsonData();
            var helper = HttpContext.RequestServices.GetService<DadaHelper>();
            var flagId = int.Parse(Request.Query["flagId"].First());
            var reason = Request.Query["reason"].First();

            var back = await helper.CancelOrderAsync(order.OrderCode, Business, flagId, reason);
            //取消不成功
            if (!back.IsSuccess())
            {
                result.Msg = back.msg;
                return result;
            }
            result.Success = Service.CancelSuccess(order, back);
            result.Data = OrderStatus.CallOff;
            result.Success = true;
            result.Msg = "配送取消成功";
            Service.Commit();
            return result;
        }
        private async Task<JsonData> DwdCancel(Order order)
        {
            var result = new JsonData();
            var helper = HttpContext.RequestServices.GetService<DwdHelper>();
            var reason = Request.Query["reason"].First();
            var back = await helper.CancelOrderAsync(order, reason);
            //取消不成功
            if (!back.success)
            {
                result.Msg = back.message;
                return result;
            }
            order.Status = OrderStatus.CallOff;
            result.Data = OrderStatus.CallOff;
            result.Success = true;
            result.Msg = "配送取消成功";
            Service.Commit();
            return result;
        }
        private async Task<JsonData> YcfkCancel(Order order)
        {
            var result = new JsonData();
            var helper = YcfkHelper.GetHelper();
            var reason = Request.Query["reason"].First() + "";
            var json = await helper.Cancel(order.OrderCode + "_" + order.DistributionFlow, reason, Business.YcfkKey, Business.YcfkSecret);
            var jObj = JObject.Parse(json);
            var code = jObj["StateCode"].Value<int>();
            if (code > 0)
            {
                result.Msg = jObj["StateMsg"].Value<string>();
                order.ErrorReason = result.Msg;
                return result;
            }
            order.Status = OrderStatus.CallOff;
            result.Data = OrderStatus.CallOff;
            result.Success = true;
            result.Msg = "配送取消成功";
            Service.Commit();
            return result;
        }

        private DWDStore GetDwdShop()
        {
            var shop = Business.DWDStore;
            if (shop != null) return shop;
            shop = Service.GetDwdShop(Business.ID);
            if (shop == null) return null;
            Business.DWDStore = shop;
            HttpContext.Session.Set(AppData.Session, Business);
            return shop;
        }

    }
}