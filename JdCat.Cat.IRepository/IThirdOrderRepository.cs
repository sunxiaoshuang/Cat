using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
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
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> AddOrderNotifyAsync(ThirdOrder order);


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
        /// 获取订单列表
        /// </summary>
        /// <param name="source">订单来源（99表示所有来源订单）</param>
        /// <param name="start">下单开始时间</param>
        /// <param name="end">下单截止时间</param>
        /// <param name="paging">分页参数</param>
        /// <returns></returns>
        Task<List<ThirdOrder>> GetOrdersAsync(int source, DateTime start, DateTime end, PagingQuery paging);
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ThirdOrder> GetOrderDetailAsync(int id);

    }
}
