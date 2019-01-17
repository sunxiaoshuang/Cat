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
    public class DwdRepository : BaseRepository<DWDStore>, IDwdRepository
    {
        public DwdRepository(CatDbContext context) : base(context)
        {

        }

        public bool CreateShop(DWDStore store)
        {
            // 首先删除当前的商户
            Context.Database.ExecuteSqlCommand("delete from `DWDStore` where `BusinessId`={0}", store.BusinessId);
            // 然后保存
            Context.DWDStores.Add(store);
            return Context.SaveChanges() > 0;
        }

        public bool AddRecharge(DWD_Recharge recharge)
        {
            Context.DWD_Recharges.Add(recharge);
            return Context.SaveChanges() > 0;
        }

        public void RechargeSuccess(string bizNo)
        {
            var entity = Context.DWD_Recharges.FirstOrDefault(a => a.DwdCode == bizNo);
            if (entity.IsFinish) return;
            entity.IsFinish = true;
            Context.SaveChanges();
        }

    }
}
