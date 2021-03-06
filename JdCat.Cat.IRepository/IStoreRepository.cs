﻿using System;
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
        Task<List<TangOrder>> GetOrdersAsync(int businessId, PagingQuery paging, DateTime startDate, DateTime endDate, string code = null);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TangOrder> GetOrderAsync(int id);
        /// <summary>
        /// 获取商户支付方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Model.Data.PaymentType>> GetPaymentsAsync(int id);
        /// <summary>
        /// 更新订单支付方式
        /// </summary>
        /// <param name="id"></param>
        /// <param name="payments"></param>
        /// <returns></returns>
        Task UpdateOrderPaymentsAsync(int id, IEnumerable<TangOrderPayment> payments);
        /// <summary>
        /// 获取堂食菜单（简单版）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<object> GetSimpleStoreProductsAsync(int id);
        /// <summary>
        /// 订单商品退货
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<TangOrderProduct> RetTangProductAsync(TangOrderProduct product);


        /// <summary>
        /// 获取商户厨师报表
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetCooksReportAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户厨师报表（外卖）
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetCooksReportForTakeoutAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个厨师报表
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleCookReportAsync(int cookId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个厨师报表（外卖）
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleCookReportForTakeoutAsync(int cookId, DateTime start, DateTime end);
        /// <summary>
        /// 获取厨师的产出详情
        /// </summary>
        /// <param name="cookIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetCookDetailReportAsync(IEnumerable<int> cookIds, DateTime start, DateTime end);
        /// <summary>
        /// 获取厨师产出详情（外卖）
        /// </summary>
        /// <param name="cookIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetCookDetailReportForTakeoutAsync(IEnumerable<int> cookIds, DateTime start, DateTime end);
        /// <summary>
        /// 获取第三方订单的厨师报表
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetThirdCooksReportAsync(int businessId, DateTime start, DateTime end, int type);
        /// <summary>
        /// 获取第三方订单中单个厨师的统计数据
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleThirdCookReportAsync(int cookId, DateTime start, DateTime end, int type);
        /// <summary>
        /// 获取第三方订单的厨师产出详情
        /// </summary>
        /// <param name="cookIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetThirdCookDetailReportAsync(IEnumerable<int> cookIds, DateTime start, DateTime end, int type);

        /// <summary>
        /// 获取商户档口报表
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetBoothsReportAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户档口报表（外卖）
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetBoothsReportForTakeoutAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个档口报表
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleBoothReportAsync(int boothId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个档口报表（外卖）
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleBoothReportForTakeoutAsync(int boothId, DateTime start, DateTime end);
        /// <summary>
        /// 获取档口产出详情
        /// </summary>
        /// <param name="boothIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetBoothDetailReportAsync(IEnumerable<int> boothIds, DateTime start, DateTime end);
        /// <summary>
        /// 获取档口产出详情（外卖）
        /// </summary>
        /// <param name="boothIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetBoothDetailReportForTakeoutAsync(IEnumerable<int> boothIds, DateTime start, DateTime end);

        /// <summary>
        /// 获取第三方订单档口统计
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetThirdBoothsReportAsync(int businessId, DateTime start, DateTime end, int type);
        /// <summary>
        /// 获取单个档口的第三方订单统计
        /// </summary>
        /// <param name="boothId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetSingleThirdBoothReportAsync(int boothId, DateTime start, DateTime end, int type);
        /// <summary>
        /// 获取第三方订单的档口产出详情
        /// </summary>
        /// <param name="boothIds"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<Report_ProductSale>> GetThirdBoothDetailReportAsync(IEnumerable<int> boothIds, DateTime start, DateTime end, int type);

        /// <summary>
        /// 获取商户食品销售统计数据
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductRanking>> GetProductsDataAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户食品销售统计数据（外卖）
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_ProductRanking>> GetProductsDataForTakeoutAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户套餐商品数据
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Setmeal>> GetSetmealDataAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户套餐商品数据（外卖）
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Setmeal>> GetSetmealDataForTakeoutAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户优惠统计
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Benefit>> GetBenefitDataAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户在指定时间内，单个优惠方式的订单详情
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<object> GetSingleBenetifDataAsync(int businessId, string name, DateTime start, DateTime end);
        /// <summary>
        /// 获取商户支付方式汇总数据
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<Report_Payment>> GetPaymentDataAsync(int businessId, DateTime start, DateTime end);
        /// <summary>
        /// 获取单个支付方式所关联的订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<object> GetSinglePaymentDataAsync(int id, DateTime start, DateTime end);
    }
}
