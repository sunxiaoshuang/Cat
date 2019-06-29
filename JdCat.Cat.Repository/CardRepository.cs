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

    }
}
