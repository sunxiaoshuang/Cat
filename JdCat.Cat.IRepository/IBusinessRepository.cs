using System;
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
        /// 根据商户编码获取商户对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Business GetBusinessByStoreId(string code);
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
        Task<bool> BindPrintDevice(Business business, FeyinDevice device);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="business"></param>
        bool UpdatePassword(Business business);
        /// <summary>
        /// 设置商户的默认打印机
        /// </summary>
        /// <param name="business"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool SetDefaultPrinter(Business business, int id);
        /// <summary>
        /// 获取监听的用户
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        List<WxListenUser> GetWxListenUser(int businessId);
        /// <summary>
        /// 绑定微信监听
        /// </summary>
        /// <param name="user">微信用户</param>
        void BindWxListen(WxListenUser user);
        /// <summary>
        /// 保存二维码地址
        /// </summary>
        /// <param name="business"></param>
        void SaveWxQrcode(Business business);
        /// <summary>
        /// 移除微信服务通知用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void RemoveWxListenUser(int id);

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
        /// <summary>
        /// 前十
        /// </summary>
        /// <param name="business"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        List<Report_ProductPrice> GetProductPriceTop10(Business business, DateTime date);
        /// <summary>
        /// 获取指定日期的销售统计数据
        /// </summary>
        /// <param name="business"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<Report_SaleStatistics> GetSaleStatistics(Business business, DateTime start, DateTime end);

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
        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="saleCoupon"></param>
        /// <returns></returns>
        JsonData CreateCoupon(SaleCoupon saleCoupon);
        /// <summary>
        /// 根据id获取优惠券实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SaleCoupon GetCouponById(int id);
        /// <summary>
        /// 获取商户优惠券列表
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<SaleCoupon> GetCoupon(Business business, bool tracking = false);
        /// <summary>
        /// 获取商户截止到当前可用的优惠券
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        List<SaleCoupon> GetCouponValid(Business business, bool tracking = false);
        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonData DeleteCoupon(int id);
        /// <summary>
        /// 下架优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonData DownCoupon(int id);
        /// <summary>
        /// 根据商户获取折扣活动
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<SaleProductDiscount> GetDiscounts(Business business);
        /// <summary>
        /// 创建折扣活动
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        JsonData CreateDiscount(SaleProductDiscount discount);
        /// <summary>
        /// 创建折扣活动
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<SaleProductDiscount> CreateDiscount(List<SaleProductDiscount> list);
        /// <summary>
        /// 删除折扣活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonData DeleteDiscount(int id);
        /// <summary>
        /// 修改折扣活动
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        JsonData UpdateDiscount(SaleProductDiscount discount);

        #endregion

        #region 客户端请求
        Business Login(string username, string password);
        #endregion


    }
}
