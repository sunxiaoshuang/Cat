using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Repository
{
    public class CardRepository : BaseRepository<WxCard>, ICardRepository
    {
        public CardRepository(CatDbContext context) : base(context)
        {

        }

        public async Task<List<WxCard>> GetCardsAsync(int businessId)
        {
            return await Context.WxCards.Where(a => a.BusinessId == businessId && a.Status == EntityStatus.Normal).ToListAsync();
        }

        public async Task<WxCard> GetCardAsync(string cardId)
        {
            return await Context.WxCards.FirstOrDefaultAsync(a => a.CardId == cardId);
        }

        public async Task<WxMember> GetMemberAsync(string cardId, string openId)
        {
            return await Context.WxMembers.FirstOrDefaultAsync(a => a.CardId == cardId && a.OpenId == openId);
        }

        public async Task<PaymentTarget> GetPaymentTargetAsync(string code)
        {
            return await Context.PaymentTargets.FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task<PaymentTarget> CreatePaymentTargetAsync(int id)
        {
            var prefix = "66";
            string code;
            do
            {
                code = prefix + UtilHelper.RandNum(16);
            } while ((await Context.PaymentTargets.CountAsync(a => a.Code == code)) > 0);


            var payment = new PaymentTarget { Code = code, ObjectId = id };
            await Context.AddAsync(payment);
            await Context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<CardChargeRule>> GetCardChargesAsync(int id)
        {
            return await Context.CardChargeRules.Where(a => a.WxCardId == id).ToListAsync();
        }

        public async Task SaveCardChargesAsync(IEnumerable<CardChargeRule> charges)
        {
            var entities = await Context.CardChargeRules.Where(a => a.WxCardId == charges.ElementAt(0).WxCardId).ToListAsync();
            Context.RemoveRange(entities);
            await Context.AddRangeAsync(charges);
            await Context.SaveChangesAsync();
        }

        public async Task<List<CardBonusRule>> GetCardBonusAsync(int id)
        {
            return await Context.CardBonusRules.Where(a => a.WxCardId == id).ToListAsync();
        }

        public async Task SaveCardBonusAsync(IEnumerable<CardBonusRule> charges)
        {
            var entities = await Context.CardBonusRules.Where(a => a.WxCardId == charges.ElementAt(0).WxCardId).ToListAsync();
            Context.RemoveRange(entities);
            await Context.AddRangeAsync(charges);
            await Context.SaveChangesAsync();
        }

        public async Task<ChargeRecord> ChargeSuccessAsync(WxPaySuccess result)
        {
            var record = await Context.ChargeRecords.FirstOrDefaultAsync(a => a.Code == result.out_trade_no);
            if (record == null) return null;
            var member = await Context.WxMembers.FirstAsync(a => a.ID == record.RelativeId);

            // 充值记录
            record.PayTime = DateTime.Now;
            record.Status = 1;
            record.WxPayCode = result.transaction_id;

            // 余额更新
            member.RechargeAmount += record.Amount;
            member.ChargeTimes++;
            member.GiveAmount += record.Give;
            member.Balance += record.Amount + record.Give;

            // 积分更新
            member.Bonus += record.Bonus;

            await Context.SaveChangesAsync();

            return record;
        }

        public async Task<List<ChargeRecord>> GetChargeRecordsAsync(int id, PagingQuery paging)
        {
            return await Context.ChargeRecords.Where(a => a.RelativeId == id && a.Status == 1).OrderByDescending(a => a.ID).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();
        }

    }
}
