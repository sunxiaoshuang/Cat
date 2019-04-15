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
using JdCat.Cat.Model.Report;

namespace JdCat.Cat.IRepository
{
    public interface IStoreRepository : IBaseRepository<TangOrder>
    {
        /// <summary>
        /// 获取堂食订单列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="paging"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<List<TangOrder>> GetOrdersAsync(int businessId, PagingQuery paging, DateTime startDate, DateTime endDate);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TangOrder> GetOrderAsync(int id);
        /// <summary>
        /// 获取商户厨师报表
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Cook>> GetCooksReportAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个厨师报表
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Cook>> GetSingleCookReportAsync(int cookId, DateTime start, DateTime end);
    }
}
