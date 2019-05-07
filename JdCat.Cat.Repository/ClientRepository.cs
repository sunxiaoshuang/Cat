using System;
using System.Collections.Generic;
using System.Dynamic;
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
    public class ClientRepository : BaseRepository<TangOrder>, IClientRepository
    {
        public ClientRepository(CatDbContext context) : base(context)
        {

        }

        public int UploadData<T>(IEnumerable<T> list) where T : BaseEntityClient
        {
            foreach (var item in list)
            {
                item.SyncTime = DateTime.Now;
                if (item.ID > 0)
                {
                    Context.Attach(item).State = EntityState.Modified;
                }
                else
                {
                    Context.Add(item);
                }
            }
            return Context.SaveChanges();
        }
    
        public int UploadOrder(IEnumerable<TangOrder> list)
        {
            var paymentIds = list.Select(a => a.PaymentTypeId).Distinct();
            var paymentTypes = Context.PaymentTypes.Where(a => paymentIds.Contains(a.ID)).ToList();
            foreach (var item in list)
            {
                item.PaymentTypeId = paymentTypes.FirstOrDefault(a => a.ID == item.PaymentTypeId).ID;
                if (item.StaffId == 0) item.StaffId = null;
            }
            return UploadData(list);
        }

        public int UploadOrderProducts(IEnumerable<TangOrderProduct> list)
        {
            var orderIds = list.Select(a => a.OrderObjectId).ToList();
            var orders = Context.TangOrders.Where(a => orderIds.Contains(a.ObjectId)).Select(a => new { a.ID, a.ObjectId }).ToList();
            orders.ForEach(order => {
                var products = list.Where(a => a.OrderObjectId == order.ObjectId).ToList();
                products.ForEach(a => a.OrderId = order.ID);
            });
            return UploadData(list);
        }

        public async Task<dynamic> GetSynchronousDataAsync(int businessId)
        {
            dynamic data = new ExpandoObject();
            /*
             * 1. 同步岗位信息
             * 2. 同步员工信息
             * 3. 同步支付类型
             * 4. 同步餐台信息
             * 5. 同步系统备注
             * 6. 同步打印机
             * 7. 同步菜单
             * 8. 同步厨师与商品关联
             * 9. 同步档口、档口与商品关系
             */
            var staffs = await Context.Staffs.AsNoTracking().Where(a => a.Status != EntityStatus.Deleted && a.BusinessId == businessId).ToListAsync();
            data.Staffs = staffs;
            data.StaffPosts = await Context.StaffPosts.AsNoTracking().Where(a => a.Status != EntityStatus.Deleted && a.BusinessId == businessId).ToListAsync();
            data.Payments = await Context.PaymentTypes.AsNoTracking().Where(a => a.Status != EntityStatus.Deleted && a.BusinessId == businessId).OrderBy(a => a.Sort).ToListAsync();
            data.DeskTypes = await Context.DeskTypes.AsNoTracking().Where(a => a.BusinessId == businessId && a.Status != EntityStatus.Deleted).OrderBy(a => a.Sort).ToListAsync();
            data.Desks = await Context.Desks.AsNoTracking().Where(a => a.Status != EntityStatus.Deleted && a.BusinessId == businessId).ToListAsync();
            data.Marks = await Context.SystemMarks.AsNoTracking().Where(a => a.BusinessId == businessId).ToListAsync();
            data.Printers = await Context.ClientPrinters.AsNoTracking().Where(a => a.BusinessId == businessId).ToListAsync();
            data.ProductTypes = await Context.ProductTypes.AsNoTracking().Where(a => a.BusinessId == businessId).OrderBy(a => a.Sort).ToListAsync();
            data.Products = await Context.Products.AsNoTracking().Include(a => a.Formats).Include(a => a.Attributes).Include(a => a.Images).Where(a => a.BusinessId == businessId).ToListAsync();
            var staffIds = staffs.Select(a => a.ID);
            data.CookProductRelatives = await Context.CookProductRelatives.AsNoTracking().Where(a => staffIds.Contains(a.StaffId)).ToListAsync();
            var booths = await Context.StoreBooths.AsNoTracking().Where(a => a.BusinessId == businessId).ToListAsync();
            var boothIds = booths.Select(a => a.ID).ToList();
            data.Booths = booths;
            data.BoothProductRelatives = await Context.BoothProductRelatives.AsNoTracking().Where(a => boothIds.Contains(a.StoreBoothId)).ToListAsync();
            return data;
        }

    }
}
