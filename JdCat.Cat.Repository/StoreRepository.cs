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
using JdCat.Cat.Model.Report;
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

        public async Task<List<TangOrder>> GetOrdersAsync(int businessId, PagingQuery paging, DateTime startTime, DateTime endTime)
        {
            var end = endTime.AddDays(1);
            var query = Context.TangOrders.Where(a => a.BusinessId == businessId && a.CreateTime >= startTime && a.CreateTime < end);
            paging.RecordCount = query.Count();
            return await query.OrderByDescending(a => a.CreateTime).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();
        }

        public async Task<TangOrder> GetOrderAsync(int id)
        {
            return await Context.TangOrders.Include(a => a.TangOrderProducts).FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<List<Report_Cook>> GetCooksReportAsync(int businessId, DateTime start, DateTime end)
        {
            var staffQuery = from staff in Context.Staffs
                             join post in Context.StaffPosts on staff.StaffPostId equals post.ID
                             where staff.BusinessId == businessId && (post.Authority & StaffPostAuth.Cook) > 0
                             select new Report_Cook { Id = staff.ID, Name = staff.Name };
            var staffs = await staffQuery.ToListAsync();
            if (staffs == null || staffs.Count == 0) return null;
            var ids = staffs.Select(a => a.Id).ToList();

            var productQuery = from relative in Context.CookProductRelatives
                               join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId
                               where ids.Contains(relative.StaffId) && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                               select new { relative.StaffId, product.Quantity, product.Amount };

            var products = await productQuery.ToListAsync();

            staffs.ForEach(a => {
                var p = products.Where(b => a.Id == b.StaffId).ToList();
                if (p.Count == 0) return;
                a.Count = p.Sum(c => c.Quantity);
                a.Amount = p.Sum(c => c.Amount);
            });
            return staffs;
        }

        public async Task<List<Report_Cook>> GetSingleCookReportAsync(int cookId, DateTime start, DateTime end)
        {
            var query = from relative in Context.CookProductRelatives
                        join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId into joinProduct
                        from product in joinProduct.DefaultIfEmpty()
                        where relative.StaffId == cookId && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                        group product by new { product.ProductId, product.Name } into g
                        select new Report_Cook
                        {
                            Id = g.Key.ProductId,
                            Name = g.Key.Name,
                            Count = g.Sum(a => a.Quantity),
                            Amount = g.Sum(a => a.Amount)
                        };
            return await query.OrderByDescending(a => a.Amount).ToListAsync();
        }







    }
}
