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
    public class DwdRepository : BaseRepository<DWD_Business>, IDwdRepository
    {
        public DwdRepository(CatDbContext context) : base(context)
        {

        }

        public bool CreateShop(DWD_Business business)
        {
            // 首先删除当前的商户
            Context.Database.ExecuteSqlCommand("delete dbo.DWDBusiness where BusinessId={0}", business.ID);
            // 然后保存
            Context.DWD_Businesses.Add(business);
            return Context.SaveChanges() > 0;
        }

    }
}
