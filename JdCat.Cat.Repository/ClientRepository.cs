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
            var paymentTypeIds = list.Select(a => a.PaymentTypeObjectId).Distinct();
            var paymentTypes = Context.PaymentTypes.Where(a => paymentTypeIds.Contains(a.ObjectId)).ToList();
            foreach (var item in list)
            {
                item.PaymentTypeId = paymentTypes.FirstOrDefault(a => a.ObjectId == item.PaymentTypeObjectId).ID;
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

    }
}
