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
    public interface IUtilRepository : IBaseRepository<Business>
    {

        #region 微信

        /// <summary>
        /// 处理微信回调事件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task<object> WxMsgHandlerAsync(WxEvent e);
        /// <summary>
        /// 根据appid、secret获取token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        Task<string> GetTokenAsync(string appId, string secret);
        /// <summary>
        /// 获取开放平台Ticket
        /// </summary>
        /// <returns></returns>
        Task<string> GetOpenTicketAsync();
        /// <summary>
        /// 设置开放平台Ticket
        /// </summary>
        void SetOpenTicketAsync(string ticket);
        /// <summary>
        /// 获取开放平台访问Token
        /// </summary>
        /// <returns></returns>
        Task<string> GetOpenTokenAsync();
        /// <summary>
        /// 获取开放平台授权访问令牌
        /// </summary>
        /// <param name="appId">公众号appid</param>
        /// <returns></returns>
        Task<string> GetAuthorizerAccessTokenAsync(string appId);
        /// <summary>
        /// 保存授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task SaveAuthorizerInfoAsync(OpenAuthInfo info);
        /// <summary>
        /// 根据商户id获取商户的授权信息
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<OpenAuthInfo> GetAuthInfoByBusinessIdAsync(int businessId);
        /// <summary>
        /// 发送订单退款通知（公众号）
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task SendRefundMsgAsync(Order order);
        /// <summary>
        /// 发送订单支付成功消息（小程序）
        /// </summary>
        /// <param name="order"></param>
        Task SendPaySuccessMsgAsync(Order order);
        /// <summary>
        /// 发送新订单提醒（公众号）
        /// </summary>
        /// <param name="order"></param>
        Task SendNewOrderMsgAsync(Order order);

        #endregion

        /// <summary>
        /// 获取下一个消费返券的编码
        /// </summary>
        /// <returns></returns>
        Task<string> GetNextCodeForReturnCouponAsync();


        #region 测试

        Task<long> GetNumberAsync();

        #endregion
    }
}
