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
    public class StoreRepository : BaseRepository<TangOrder>, IStoreRepository
    {
        public StoreRepository(CatDbContext context) : base(context)
        {
        }

        public List<TangOrder> GetOrders(int businessId, PagingQuery paging, DateTime startTime, DateTime endTime)
        {
            var end = endTime.AddDays(1);
            var query = Context.TangOrders.Where(a => a.BusinessId == businessId && a.CreateTime >= startTime && a.CreateTime < end);
            paging.RecordCount = query.Count();
            return query.OrderByDescending(a => a.CreateTime).Skip(paging.Skip).Take(paging.PageSize).ToList();
        }

        public TangOrder GetOrder(int id)
        {
            return Context.TangOrders.Include(a => a.TangOrderProducts).FirstOrDefault(a => a.ID == id);
        }




    }
}
