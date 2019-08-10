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
using Newtonsoft.Json.Linq;

namespace JdCat.Cat.IRepository
{
    public interface ICardRepository : IBaseRepository<WxCard>
    {
        /// <summary>
        /// 获取指定商户的所有卡券
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<List<WxCard>> GetCardsAsync(int businessId);
        /// <summary>
        /// 获取会员卡信息
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        Task<WxCard> GetCardAsync(string cardId);
        /// <summary>
        /// 根据会员卡id与openid回去会员信息
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<WxMember> GetMemberAsync(string cardId, string openId);
        /// <summary>
        /// 根据编码获取支付对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<PaymentTarget> GetPaymentTargetAsync(string code);
        /// <summary>
        /// 根据关联对象id生成支付对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PaymentTarget> CreatePaymentTargetAsync(int id);
        /// <summary>
        /// 获取会员储值规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<CardChargeRule>> GetCardChargesAsync(int id);
        /// <summary>
        /// 保存储值规则
        /// </summary>
        /// <param name="charges"></param>
        /// <returns></returns>
        Task SaveCardChargesAsync(IEnumerable<CardChargeRule> charges);
        /// <summary>
        /// 获取会员卡积分规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<CardBonusRule>> GetCardBonusAsync(int id);
        /// <summary>
        /// 保存会员卡积分规则
        /// </summary>
        /// <param name="charges"></param>
        /// <returns></returns>
        Task SaveCardBonusAsync(IEnumerable<CardBonusRule> charges);
        /// <summary>
        /// 会员充值成功
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<ChargeRecord> ChargeSuccessAsync(WxPaySuccess result);
        /// <summary>
        /// 根据会员id获取充值记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<List<ChargeRecord>> GetChargeRecordsAsync(int id, PagingQuery paging);
    }
}
