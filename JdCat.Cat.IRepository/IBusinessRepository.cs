﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Report;

namespace JdCat.Cat.IRepository
{
    public interface IBusinessRepository : IBaseRepository<Business>
    {
        /// <summary>
        /// 获取商户对象，不要跟踪状态
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Business GetBusiness(Expression<Func<Business, bool>> expression);
        /// <summary>
        /// 保存商户基本信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveBase(Business business);
        bool ChangeAutoReceipt(Business business, bool state);
        bool ChangeClose(Business business, bool state);
        /// <summary>
        /// 保存小程序信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveSmall(Business business);
        /// <summary>
        /// 保存达达配置信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveDada(Business business);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="businessId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<string> UploadAsync(string url, int businessId, string source);
        /// <summary>
        /// 保存飞印商户编码与apikey
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveFeyin(Business business);
        /// <summary>
        /// 获取商户打印机列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        IEnumerable<FeyinDevice> GetPrinters(int businessId);
        /// <summary>
        /// 绑定打印设备
        /// </summary>
        /// <param name="business"></param>
        /// <param name="device"></param>
        bool BindPrintDevice(Business business, FeyinDevice device);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="business"></param>
        bool UpdatePassword(Business business);
        /// <summary>
        /// 设置默认打印机编码
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SetDefaultPrinter(Business business);

        #region 报表类方法
        /// <summary>
        /// 获取指定日期内商户的每日订单统计
        /// </summary>
        /// <param name="business"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<Report_Order> GetOrderTotal(Business business, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取指定日期内的商品统计
        /// </summary>
        /// <param name="business"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        List<Report_Product> GetProductTop10(Business business, DateTime date);

        List<Report_ProductPrice> GetProductPriceTop10(Business business, DateTime date);

        #endregion

        #region 营销类方法
        /// <summary>
        /// 根据id获取满减实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SaleFullReduce GetFullReduceById(int id);
        /// <summary>
        /// 创建满减活动
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        JsonData CreateFullReduce(SaleFullReduce entity);
        /// <summary>
        /// 修改满减活动
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        JsonData UpdateFullReduce(SaleFullReduce entity);

        /// <summary>
        /// 获取商户满减活动列表
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<SaleFullReduce> GetFullReduce(Business business, bool tracking = true);
        /// <summary>
        /// 删除营销活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonData DeleteFullReduce(int id);


        #endregion

    }
}
