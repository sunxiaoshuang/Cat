using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class ClientController : Controller
    {
        public ILog Log
        {
            get
            {
                return LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(ClientController));
            }
        }
        public override JsonResult Json(object data)
        {
            return base.Json(data, AppData.JsonSetting);
        }
        /// <summary>
        /// 商户登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult Login(string code, string pwd, [FromServices]IBusinessRepository service)
        {
            var business = service.Login(code, UtilHelper.MD5Encrypt(pwd));
            var result = new JsonData();
            if (business == null)
            {
                result.Msg = "用户名或密码错误";
            }
            else
            {
                result.Data = business;
                result.Success = true;
            }
            return Json(result);
        }
        /// <summary>
        /// 获取商户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBusiness(int id, [FromServices]IBusinessRepository service)
        {
            return Json(new JsonData { Data = service.Get(id) });
        }
        /// <summary>
        /// 客户端数据初始化
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult InitClient(int id, [FromServices]IBusinessRepository service, [FromServices]IProductRepository productRepository)
        {
            var desk = service.GetDeskTypes(id);                // 餐桌信息
            var Types = productRepository.GetTypes(new Business { ID = id });
            return Json(new
            {
                Desk = desk,
                Types
            });
        }

        /// <summary>
        /// 客户端获取订单列表，按天查询有效的订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <param name="createTime"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult GetOrders(int id, [FromQuery]PagingQuery query, [FromQuery]DateTime? createTime, [FromServices]IOrderRepository service)
        {
            var result = new JsonData();
            var orders = service.GetOrder(id, createTime ?? DateTime.Now, query);
            result.Data = new
            {
                list = orders,                          // 订单列表
                rows = query.RecordCount                // 总记录数
            };
            result.Success = true;
            return Json(result);
        }
        /// <summary>
        /// 客户端获取订单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult GetOrderDetail(int id, [FromServices]IOrderRepository service)
        {
            var order = service.GetOrderForDetail(id);
            return Json(new JsonData { Success = true, Data = order });
        }
        /// <summary>
        /// 获取打印机列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetPrinters(int id, [FromServices]IBusinessRepository service)
        {
            var result = new JsonData();
            var printers = service.GetClientPrinters(id);
            result.Success = true;
            result.Data = printers.Count == 0 ? null : printers;
            return Json(result);
        }
        /// <summary>
        /// 保存客户端打印机列表设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult SavePrinters(int id, [FromBody]IEnumerable<ClientPrinter> printers, [FromServices]IBusinessRepository service)
        {
            var result = new JsonData();
            foreach (var item in printers)
            {
                item.BusinessId = id;
            }
            try
            {
                service.SavePrinters(printers);
                result.Success = true;
                result.Msg = "成功";
                result.Data = printers;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
            }
            return Json(result);
        }
        /// <summary>
        /// 保存客户端打印机设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult SavePrinter(int id, [FromBody]ClientPrinter printer, [FromServices]IBusinessRepository service)
        {
            var result = new JsonData();
            printer.BusinessId = id;
            try
            {
                service.SavePrinter(printer);
                result.Success = true;
                result.Msg = "成功";
                result.Data = printer;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
            }
            return Json(result);
        }
        /// <summary>
        /// 删除客户端打印机
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult DeletePrinter(int id, [FromServices]IBusinessRepository service)
        {
            var result = service.Delete(new ClientPrinter { ID = id });
            return Json(new JsonData { Success = result > 0 });
        }
        /// <summary>
        /// 获取商户菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetProducts(int id, [FromServices]IProductRepository service)
        {
            return Json(service.GetTypes(id));
        }
        /// <summary>
        /// 更新打印机关联的菜品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ids"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult PutPrinterProducts(int id, [FromQuery]string ids, [FromServices]IBusinessRepository service)
        {
            service.PutPrinterProducts(id, ids);
            return Json(new JsonData { Success = true, Msg = "ok" });
        }
        /// <summary>
        /// 保存餐桌类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult SaveDeskType([FromBody]DeskType type, [FromServices]IBusinessRepository service)
        {
            service.SaveDeskType(type);
            return Json(new JsonData
            {
                Success = true,
                Data = type
            });
        }
        /// <summary>
        /// 修改餐桌类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult UpdateDeskType([FromBody]DeskType type, [FromServices]IBusinessRepository service)
        {
            service.UpdateDeskType(type);
            return Json(new JsonData
            {
                Success = true,
                Data = type
            });
        }
        /// <summary>
        /// 删除餐桌类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult DeleteDeskType(int id, [FromServices]IBusinessRepository service)
        {
            service.DeleteDeskType(id);
            return Json(new JsonData
            {
                Success = true,
                Msg = "删除成功"
            });
        }
        /// <summary>
        /// 保存餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult SaveDesk([FromBody]Desk desk, [FromServices]IBusinessRepository service)
        {
            service.SaveDesk(desk);
            return Json(new JsonData
            {
                Success = true,
                Data = desk
            });
        }
        /// <summary>
        /// 修改餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult UpdateDesk([FromBody]Desk desk, [FromServices]IBusinessRepository service)
        {
            service.UpdateDesk(desk);
            return Json(new JsonData
            {
                Success = true,
                Data = desk
            });
        }
        /// <summary>
        /// 删除餐桌
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public IActionResult DeleteDesk(int id, [FromServices]IBusinessRepository service)
        {
            service.DeleteDesk(id);
            return Json(new JsonData
            {
                Success = true,
                Msg = "删除成功"
            });
        }

        #region 上传数据接口
        public IActionResult UploadStaff([FromBody]IEnumerable<Staff> list, [FromServices]IClientRepository service)
        {
            int count = 0;
            if (list != null) count = service.UploadData(list);
            return Json(new JsonData
            {
                Success = true,
                Msg = $"更新员工记录：{count}条。",
                Data = list?.Select(a => new { a.ID, a.ObjectId })
            });
        }
        public IActionResult UploadPaymentType([FromBody]IEnumerable<PaymentType> list, [FromServices]IClientRepository service)
        {
            int count = 0;
            if (list != null) count = service.UploadData(list);
            return Json(new JsonData
            {
                Success = true,
                Msg = $"更新支付类型记录：{count}条。",
                Data = list?.Select(a => new { a.ID, a.ObjectId })
            });
        }
        public IActionResult UploadTangOrder([FromBody]IEnumerable<TangOrder> list, [FromServices]IClientRepository service)
        {
            int count = 0;
            if (list != null) count = service.UploadOrder(list);
            return Json(new JsonData
            {
                Success = true,
                Msg = $"更新订单记录：{count}条。",
                Data = list?.Select(a => new { a.ID, a.ObjectId })
            });
        }
        public IActionResult UploadTangOrderProduct([FromBody]IEnumerable<TangOrderProduct> list, [FromServices]IClientRepository service)
        {
            int count = 0;
            if (list != null) count = service.UploadOrderProducts(list);
            return Json(new JsonData
            {
                Success = true,
                Msg = $"更新订单商品记录：{count}条。",
                Data = list?.Select(a => new { a.ID, a.ObjectId })
            });
        }
        #endregion

    }
}
