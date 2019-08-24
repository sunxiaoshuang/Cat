using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Report;
using Newtonsoft.Json.Linq;

namespace JdCat.Cat.IRepository
{
    public interface IThirdOrderRepository : IBaseRepository<ThirdOrder>
    {
        /// <summary>
        /// 根据appid获取美团应用的secret
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<string> GetMTAppKeyAsync(string appId);
        /// <summary>
        /// 根据第三方订单id获取订单
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        Task<ThirdOrder> GetOrderByCodeAsync(string order_id);
        /// <summary>
        /// 保存美团订单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<ThirdOrder> MT_SaveAsync(Dictionary<string, string> dic);
        /// <summary>
        /// 完成美团订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ThirdOrder> MT_FinishAsync(string id);
        /// <summary>
        /// 取消美团订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<ThirdOrder> MT_CancelAsync(string id, string reason);
        /// <summary>
        /// 根据美团门店编号获取商户信息
        /// </summary>
        /// <param name="poi"></param>
        /// <returns></returns>
        Task<Business> GetBusinessByMtPoi(string poi);

        /// <summary>
        /// 根据应用Appid获取饿了么应用密钥
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<string> GetElemeAppSecretAsync(long appId);
        /// <summary>
        /// 保存饿了么订单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ThirdOrder> ElemeSaveAsync(JObject message);
        /// <summary>
        /// 饿了么接单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ThirdOrder> ElemeReceivedAsync(JObject message);
        /// <summary>
        /// 饿了么完成订单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ThirdOrder> ElemeFinishAsync(JObject message);
        /// <summary>
        /// 饿了么取消订单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ThirdOrder> ElemeCancelAsync(JObject message);

        /// <summary>
        /// 新增订单通知
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="isTimes">是否多次打印</param>
        /// <returns></returns>
        Task<bool> AddOrderNotifyAsync(ThirdOrder order, bool isTimes = false);


        /// <summary>
        /// 获取饿了么访问token
        /// </summary>
        /// <param name="url">token获取url</param>
        /// <param name="appKey">应用KEY</param>
        /// <param name="appSecret">应用SECRET</param>
        /// <returns></returns>
        Task<string> GetElemeTokenAsync(string url, string appKey, string appSecret);
        /// <summary>
        /// 获取商户与第三方平台的商品映射关系
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<ThirdProductMapping>> GetProductMappingsAsync(int businessId, int source);
        /// <summary>
        /// 清空商品映射
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task ClearMappingsAsync(int businessId, int source);
        /// <summary>
        /// 设置商品映射
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        Task SetProductMappingsAsync(IEnumerable<ThirdProductMapping> mappings);

        /// <summary>
        /// 一城飞客配送状态变更
        /// </summary>
        /// <param name="ycfk"></param>
        /// <returns></returns>
        Task UpdateOrderStatus(YcfkCallback ycfk);

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="businessId">商户id</param>
        /// <param name="source">订单来源（99表示所有来源订单）</param>
        /// <param name="start">下单开始时间</param>
        /// <param name="end">下单截止时间</param>
        /// <param name="paging">分页参数</param>
        /// <param name="dayNum">当日编号</param>
        /// <returns></returns>
        Task<List<ThirdOrder>> GetOrdersAsync(int businessId, int source, DateTime start, DateTime end, PagingQuery paging, int dayNum);
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ThirdOrder> GetOrderDetailAsync(int id);

        /// <summary>
        /// 获取商户指定时间内的商品统计
        /// </summary>
        /// <param name="businessId">商户id</param>
        /// <param name="source">订单来源，99代表不区分来源</param>
        /// <param name="start">查询开始时间</param>
        /// <param name="end">查询结束时间</param>
        /// <returns></returns>
        Task<List<Report_ProductRanking>> GetProductsDataAsync(int businessId, int source, DateTime start, DateTime end);

    }
}
