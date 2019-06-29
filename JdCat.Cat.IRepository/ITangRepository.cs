using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;

namespace JdCat.Cat.IRepository
{
    public interface ITangRepository : IBaseRepository<Staff>
    {
        /// <summary>
        /// 获取商户下所有的岗位
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<StaffPost>> GetStaffPostsAsync(int businessId);
        /// <summary>
        /// 商户下是否存在指定的岗位
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> IsExistPostAsync(int businessId, StaffPost post);
        /// <summary>
        /// 修改岗位
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        Task<int> UpdateStaffPostAsync(StaffPost post);
        /// <summary>
        /// 刪除岗位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<JsonData> DeleteStaffPostAsync(int id);

        /// <summary>
        /// 获取商户下的所有员工
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<Staff>> GetStaffsAsync(int businessId);
        /// <summary>
        /// 是否存在员工
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        Task<bool> IsExistStaffAsync(int businessId, Staff staff);
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        Task<Staff> AddStaffAsync(Staff staff);
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        Task<Staff> UpdateStaffAsync(Staff staff);
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<JsonData> DeleteStaffAsync(int id);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        Task<bool> ResetPasswordAsync(Staff staff);
        /// <summary>
        /// 获取厨师关联的菜品id
        /// </summary>
        /// <param name="cookId"></param>
        /// <returns></returns>
        Task<List<int>> GetProductIdsByCookAsync(int cookId);
        /// <summary>
        /// 获取商户所有厨师所绑定的商品id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<object> GetProductIdsWithCookAsync(int id);
        /// <summary>
        /// 厨师绑定菜品
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="relatives"></param>
        /// <returns></returns>
        Task<List<CookProductRelative>> BindProductsForCookAsync(int cookId, IEnumerable<CookProductRelative> relatives);

        /// <summary>
        /// 获取商户定义的标签
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<SystemMark>> GetMarksAsync(int businessId);
        /// <summary>
        /// 商户下是否存在指定标签
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        Task<bool> IsExistMarkAsync(SystemMark mark);

        /// <summary>
        /// 获取商户所有的支付方式
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<Model.Data.PaymentType>> GetPaymentsAsync(int businessId);
        /// <summary>
        /// 是否存在指定的支付方式
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<bool> IsExistPaymentAsync(JdCat.Cat.Model.Data.PaymentType payment);
        /// <summary>
        /// 新增支付方式
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<JdCat.Cat.Model.Data.PaymentType> AddPaymentAsync(JdCat.Cat.Model.Data.PaymentType payment);
        /// <summary>
        /// 设置支付方式的排序码
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<bool> SetPaymentSortAsync(Model.Data.PaymentType payment);
        /// <summary>
        /// 删除支付方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeletePaymentAsync(int id);

        /// <summary>
        /// 获取所有餐桌类型（包含餐桌）
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<DeskType>> GetDeskTypesAsync(int businessId);
        /// <summary>
        /// 是否存在餐桌类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> IsExistDeskTypeAsync(DeskType type);
        /// <summary>
        /// 是否存在餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        Task<bool> IsExistDeskAsync(Desk desk);
        /// <summary>
        /// 删除餐桌类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<JsonData> DeleteDeskTypeAsync(int id);
        /// <summary>
        /// 删除餐桌
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteDeskAsync(int id);
        /// <summary>
        /// 批量更新餐桌类别
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        Task<bool> UpdateDeskTypesAsync(IEnumerable<DeskType> types);
        /// <summary>
        /// 更新餐桌
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        Task<Desk> UpdateDeskAsync(Desk desk);

        /// <summary>
        /// 获取店内打印机
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<ClientPrinter>> GetPrintersAsync(int businessId);

        /// <summary>
        /// 获取商户档口列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<StoreBooth>> GetBoothsAsync(int businessId);
        /// <summary>
        /// 档口绑定产品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relatives"></param>
        /// <returns></returns>
        Task<List<BoothProductRelative>> BindProductsForBoothAsync(int id, IEnumerable<BoothProductRelative> relatives);
        /// <summary>
        /// 根据档口获取获取关联的商品id列表
        /// </summary>
        /// <param name="boothId"></param>
        /// <returns></returns>
        Task<List<int>> GetProductIdsByBoothAsync(int boothId);
        /// <summary>
        /// 获取商户所有档口所绑定的商品id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<object> GetProductIdsWithBusinessBoothAsync(int businessId);
    }
}
