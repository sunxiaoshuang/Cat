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
    /// <summary>
    /// 堂食管理
    /// </summary>
    public class TangController : BaseController<ITangRepository, Staff>
    {
        public TangController(AppData appData, ITangRepository service) : base(appData, service)
        {
        }

        #region 岗位管理

        /// <summary>
        /// 岗位管理
        /// </summary>
        /// <returns></returns>
        public IActionResult StaffPost()
        {
            return View();
        }
        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetStaffPosts()
        {
            var staffPosts = await Service.GetStaffPostsAsync(Business.ID);
            return Json(new { staffPosts });
        }
        /// <summary>
        /// 添加岗位
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddStaffPost([FromBody]StaffPost post)
        {
            if (await Service.IsExistPostAsync(Business.ID, post))
            {
                return Json(new JsonData { Msg = $"已存在名为【{post.Name}】的岗位，请选用其他名称", Success = false });
            }
            post.BusinessId = Business.ID;
            post.Status = EntityStatus.Normal;
            await Service.AddAsync(post);
            return Json(new JsonData { Msg = "新增成功", Success = true, Data = post });
        }
        /// <summary>
        /// 编辑岗位
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public IActionResult EditStaffPost()
        {
            return PartialView();
        }
        /// <summary>
        /// 修改岗位
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateStaffPost([FromBody]StaffPost post)
        {
            if (await Service.IsExistPostAsync(Business.ID, post))
            {
                return Json(new JsonData { Msg = $"已存在名为【{post.Name}】的岗位，请选用其他名称", Success = false });
            }
            await Service.UpdateStaffPostAsync(post);
            return Json(new JsonData { Msg = "修改成功", Success = true });
        }
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteStaffPost(int id)
        {
            return Json(await Service.DeleteStaffPostAsync(id));
        }

        #endregion

        #region 员工管理

        /// <summary>
        /// 员工管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Staff()
        {
            return View();
        }
        /// <summary>
        /// 获取商户员工列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetStaffs()
        {
            return Json(new { list = await Service.GetStaffsAsync(Business.ID) });
        }
        /// <summary>
        /// 编辑员工信息
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public IActionResult EditStaff()
        {
            return PartialView();
        }
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddStaff([FromBody]Staff staff)
        {
            if (await Service.IsExistStaffAsync(Business.ID, staff))
            {
                return Json(new JsonData { Msg = $"已存在登录名为【{staff.Alise}】的员工，请更换其他登录名", Success = false });
            }
            staff.BusinessId = Business.ID;
            await Service.AddStaffAsync(staff);
            staff.StaffPost = await Service.GetAsync<StaffPost>(staff.StaffPostId);
            return Json(new JsonData { Msg = "新增成功", Success = true, Data = staff });
        }
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateStaff([FromBody]Staff staff)
        {
            var employee = await Service.UpdateStaffAsync(staff);
            staff.StaffPost = await Service.GetAsync<StaffPost>(staff.StaffPostId);
            return Json(new JsonData { Msg = "修改成功", Success = true, Data = employee });
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            return Json(await Service.DeleteStaffAsync(id));
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ResetPassword(int id, [FromQuery]string pwd)
        {
            await Service.ResetPasswordAsync(new Staff { ID = id, Password = pwd });
            return Json(new JsonData { Success = true, Msg = "重置成功" });
        }
        /// <summary>
        /// 获取厨师关联的菜品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductIdsByCook(int id)
        {
            return Json(await Service.GetProductIdsByCookAsync(id));
        }
        /// <summary>
        /// 获取商户所有厨师所绑定的商品id
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetProductIdsWithCook()
        {
            return Json(await Service.GetProductIdsWithCookAsync(Business.ID));
        }
        /// <summary>
        /// 厨师绑定菜品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relatives"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BindProductsForCook(int id, [FromBody]IEnumerable<CookProductRelative> relatives)
        {
            await Service.BindProductsForCookAsync(id, relatives);
            return Json(new JsonData { Success = true, Msg = "绑定成功" });
        }

        #endregion

        #region 标签管理

        /// <summary>
        /// 标签
        /// </summary>
        /// <returns></returns>
        public IActionResult Mark()
        {
            return View();
        }
        /// <summary>
        /// 获取商户各种类型标签
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetMarks()
        {
            var marks = await Service.GetMarksAsync(Business.ID);
            return Json(new
            {
                Flavor = marks.Where(a => a.Category == MarkCategory.Flavor),
                OrderMark = marks.Where(a => a.Category == MarkCategory.OrderMark),
                RefundFoodReason = marks.Where(a => a.Category == MarkCategory.RefundFoodReason),
                CancelDeliveryReason = marks.Where(a => a.Category == MarkCategory.CancelDeliveryReason),
                RefundMoneyReason = marks.Where(a => a.Category == MarkCategory.RefundMoneyReason),
                CancelOrderReason = marks.Where(a => a.Category == MarkCategory.CancelOrderReason),
                PayRemark = marks.Where(a => a.Category == MarkCategory.PayRemark),
                GoodRemark = marks.Where(a => a.Category == MarkCategory.GoodRemark)
            });
        }
        /// <summary>
        /// 新增标签
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddMark([FromBody]SystemMark mark)
        {
            mark.BusinessId = Business.ID;
            if (await Service.IsExistMarkAsync(mark))
            {
                return Json(new JsonData { Success = false, Msg = $"已存在名为{mark.Name}的标签" });
            }
            await Service.AddAsync(mark);
            return Json(new JsonData { Success = true, Msg = "新增成功", Data = mark });
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult RemoveMark(int id)
        {
            var result = new JsonData
            {
                Success = Service.Delete(new SystemMark { ID = id }) > 0
            };
            if (result.Success)
            {
                result.Msg = "删除成功";
            }
            else
            {
                result.Msg = "删除失败，请刷新后重试";
            }
            return Json(result);
        }

        #endregion

        #region 支付方式

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <returns></returns>
        public IActionResult Payment()
        {
            return View();
        }
        /// <summary>
        /// 获取商户的支付方式
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPayments()
        {
            return Json(await Service.GetPaymentsAsync(Business.ID));
        }
        public async Task<IActionResult> AddPayment([FromBody]JdCat.Cat.Model.Data.PaymentType payment)
        {
            if (await Service.IsExistPaymentAsync(payment))
            {
                return Json(new JsonData { Msg = $"已存在名为[{payment.Name}]的支付方式" });
            }
            payment.BusinessId = Business.ID;
            await Service.AddPaymentAsync(payment);
            return Json(new JsonData { Success = true, Msg = "新增成功", Data = payment });
        }
        /// <summary>
        /// 设置支付方式的排序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetPaymentSort(int id, int sort)
        {
            var result = new JsonData
            {
                Success = await Service.SetPaymentSortAsync(new Model.Data.PaymentType { ID = id, Sort = sort })
            };
            if (result.Success)
            {
                result.Msg = "修改成功";
                return Json(result);
            }
            result.Msg = "修改失败，请刷新后重试";
            return Json(result);
        }
        [HttpDelete]
        public async Task<IActionResult> RemovePayment(int id)
        {
            var result = new JsonData
            {
                Success = await Service.DeletePaymentAsync(id)
            };
            if (result.Success)
            {
                result.Msg = "删除成功";
                return Json(result);
            }
            result.Msg = "删除失败，请刷新后重试";
            return Json(result);

        }

        #endregion

        #region 餐桌

        /// <summary>
        /// 餐桌管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Desk()
        {
            return View();
        }
        /// <summary>
        /// 获取所有餐桌
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetDesks()
        {
            return Json(await Service.GetDeskTypesAsync(Business.ID));
        }
        /// <summary>
        /// 新增餐桌类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddDeskType([FromBody]DeskType type)
        {
            type.BusinessId = Business.ID;
            type.Status = EntityStatus.Normal;
            await Service.AddAsync(type);
            return Json(new JsonData { Success = true, Msg = "新增成功", Data = type });
        }
        /// <summary>
        /// 新增餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddDesk([FromBody]Desk desk)
        {
            desk.BusinessId = Business.ID;
            desk.Status = EntityStatus.Normal;
            await Service.AddAsync(desk);
            return Json(new JsonData { Success = true, Msg = "新增成功", Data = desk });
        }
        /// <summary>
        /// 更新餐台区域
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateDeskTypes([FromBody]IEnumerable<DeskType> types)
        {
            var result = new JsonData
            {
                Success = await Service.UpdateDeskTypesAsync(types)
            };
            if (!result.Success)
            {
                result.Msg = "更新失败，请刷新后重试";
                return Json(result);
            }
            result.Msg = "更新成功";
            result.Success = true;
            return Json(result);
        }
        /// <summary>
        /// 修改餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateDesk([FromBody]Desk desk)
        {
            var result = new JsonData { Success = true };
            result.Data = await Service.UpdateDeskAsync(desk);
            result.Msg = "修改成功";
            return Json(result);
        }
        /// <summary>
        /// 删除餐桌类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteDeskType(int id)
        {
            return Json(await Service.DeleteDeskTypeAsync(id));
        }
        /// <summary>
        /// 删除餐桌
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteDesk(int id)
        {
            if (!await Service.DeleteDeskAsync(id))
            {
                return Json(new JsonData { Msg = "删除失败，请刷新后重试" });
            }
            return Json(new JsonData { Msg = "删除成功", Success = true });
        }

        #endregion

        #region 打印机管理

        /// <summary>
        /// 店内打印机管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Printer()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPrinters()
        {
            return Json(await Service.GetPrintersAsync(Business.ID));
        }
        /// <summary>
        /// 新增打印机
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPrinter([FromBody]ClientPrinter printer)
        {
            printer.BusinessId = Business.ID;
            await Service.AddAsync(printer);
            return Json(printer);
        }
        /// <summary>
        /// 更新打印机信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePrinter([FromBody]ClientPrinter printer)
        {
            var count = await Service.UpdateAsync(printer, new List<string> {
                nameof(printer.Name), nameof(printer.IP), nameof(printer.Port), nameof(printer.Type), nameof(printer.State), nameof(printer.Quantity),nameof(printer.Mode), nameof(printer.Format), nameof(printer.Scope), nameof(printer.CashierName)
            });
            Log.Info($"门店{Business.Name}中的打印机{printer.Name}更新，更新后状态{printer.State}");
            return Json(new JsonData { Success = true, Msg = "更新成功" });
        }
        /// <summary>
        /// 更新打印机关联的菜品
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePrinterProducts([FromBody]ClientPrinter printer)
        {
            var count = await Service.UpdateAsync(printer, new[] { nameof(printer.FoodIds) });
            return Json(new JsonData { Success = true, Msg = "关联成功" });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeletePrinter(int id)
        {
            var result = new JsonData { Success = await Service.DeleteAsync(new ClientPrinter { ID = id }) > 0 };
            if (result.Success)
            {
                result.Msg = "删除成功";
                return Json(result);
            }
            result.Msg = "删除失败，请刷新后重试";
            return Json(result);
        }
        /// <summary>
        /// 绑定打印商品页面
        /// </summary>
        /// <returns></returns>
        public IActionResult SelectProduct()
        {
            return PartialView();
        }

        #endregion

        #region 档口管理
        /// <summary>
        /// 商户档口
        /// </summary>
        /// <returns></returns>
        public IActionResult Booth()
        {
            return View();
        }
        /// <summary>
        /// 获取商户档口列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetBooths()
        {
            var list = await Service.GetBoothsAsync(Business.ID);
            return Json(list);
        }
        /// <summary>
        /// 新增档口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AddBooth([FromBody]StoreBooth booth)
        {
            var result = new JsonData { Success = true };
            booth.BusinessId = Business.ID;
            result.Data = await Service.AddAsync(booth);
            return Json(result);
        }
        /// <summary>
        /// 更新档口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UpdateBooth([FromBody]StoreBooth booth)
        {
            var result = new JsonData
            {
                Success = await Service.UpdateAsync(booth, new List<string> { nameof(booth.Name) }) > 0
            };
            return Json(result);
        }
        /// <summary>
        /// 删除档口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteBooth(int id)
        {
            var result = new JsonData { Success = await Service.DeleteAsync(new StoreBooth { ID = id }) > 0 };
            return Json(result);
        }
        /// <summary>
        /// 获取档口绑定的商品id列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetProductIdsByBooth(int id)
        {
            return Json(await Service.GetProductIdsByBoothAsync(id));
        }
        /// <summary>
        /// 获取商户所有档口所绑定的商品id
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetProductIdsWithBusinessBooth()
        {
            return Json(await Service.GetProductIdsWithBusinessBoothAsync(Business.ID));
        }
        /// <summary>
        /// 档口绑定菜品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relatives"></param>
        /// <returns></returns>
        public async Task<IActionResult> BindProductForBooth(int id, [FromBody]List<BoothProductRelative> relatives)
        {
            var result = new JsonData
            {
                Data = await Service.BindProductsForBoothAsync(id, relatives),
                Success = true
            };
            return Json(result);
        }

        #endregion

        /// <summary>
        /// 勾选商品
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckProducts()
        {
            return PartialView();
        }

    }
}
