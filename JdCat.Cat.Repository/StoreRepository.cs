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

        public async Task<List<TangOrder>> GetOrdersAsync(int businessId, PagingQuery paging, DateTime startTime, DateTime endTime, string code = null)
        {
            var end = endTime.AddDays(1);
            var query = Context.TangOrders.Where(a => a.BusinessId == businessId && a.CreateTime >= startTime && a.CreateTime < end);
            if(!string.IsNullOrEmpty(code))
            {
                query = query.Where(a => a.Code.Contains(code));
            }
            paging.RecordCount = query.Count();
            return await query.OrderByDescending(a => a.CreateTime).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();
        }

        public async Task<TangOrder> GetOrderAsync(int id)
        {
            return await Context.TangOrders.Include(a => a.TangOrderProducts).Include(a => a.TangOrderPayments).FirstOrDefaultAsync(a => a.ID == id);
        }

        public async Task<List<Cat.Model.Data.PaymentType>> GetPaymentsAsync(int id)
        {
            return await Context.PaymentTypes.Where(a => a.BusinessId == id && a.Status == EntityStatus.Normal).ToListAsync();
        }

        public async Task<List<Report_ProductSale>> GetCooksReportAsync(int businessId, DateTime start, DateTime end)
        {
            var staffQuery = from staff in Context.Staffs
                             join post in Context.StaffPosts on staff.StaffPostId equals post.ID
                             where staff.BusinessId == businessId && (post.Authority & StaffPostAuth.Cook) > 0
                             select new Report_ProductSale { Id = staff.ID, Name = staff.Name };
            var staffs = await staffQuery.ToListAsync();
            if (staffs == null || staffs.Count == 0) return null;
            var ids = staffs.Select(a => a.Id).ToList();

            var productQuery = from relative in Context.CookProductRelatives
                               join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId
                               where ids.Contains(relative.StaffId) && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                               select new { relative.StaffId, product.Quantity, product.Amount };

            var products = await productQuery.ToListAsync();

            staffs.ForEach(a =>
            {
                var p = products.Where(b => a.Id == b.StaffId).ToList();
                if (p.Count == 0) return;
                a.Count = p.Sum(c => c.Quantity);
                a.Amount = p.Sum(c => c.Amount);
            });
            return staffs;
        }
        public async Task<List<Report_ProductSale>> GetCooksReportForTakeoutAsync(int businessId, DateTime start, DateTime end)
        {
            var staffQuery = from staff in Context.Staffs
                             join post in Context.StaffPosts on staff.StaffPostId equals post.ID
                             where staff.BusinessId == businessId && (post.Authority & StaffPostAuth.Cook) > 0
                             select new Report_ProductSale { Id = staff.ID, Name = staff.Name };
            var staffs = await staffQuery.ToListAsync();
            if (staffs == null || staffs.Count == 0) return null;
            var ids = staffs.Select(a => a.Id).ToList();

            var productQuery = from relative in Context.CookProductRelatives
                               join product in Context.OrderProducts on relative.ProductId equals product.ProductId
                               join order in Context.Orders on product.OrderId equals order.ID
                               where ids.Contains(relative.StaffId) && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                               select new { relative.StaffId, product.Quantity, product.Price };

            var products = await productQuery.ToListAsync();

            staffs.ForEach(a =>
            {
                var p = products.Where(b => a.Id == b.StaffId).ToList();
                if (p.Count == 0) return;
                a.Count = p.Sum(c => c.Quantity ?? 0);
                a.Amount = p.Sum(c => c.Price ?? 0);
            });
            return staffs;
        }

        public async Task<List<Report_ProductSale>> GetSingleCookReportAsync(int cookId, DateTime start, DateTime end)
        {
            var query = from relative in Context.CookProductRelatives
                        join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId into joinProduct
                        from product in joinProduct.DefaultIfEmpty()
                        where relative.StaffId == cookId && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                        group product by new { product.ProductId, product.Name } into g
                        select new Report_ProductSale
                        {
                            Id = g.Key.ProductId,
                            Name = g.Key.Name,
                            Count = g.Sum(a => a.Quantity),
                            Amount = g.Sum(a => a.Amount)
                        };
            return await query.OrderByDescending(a => a.Amount).ToListAsync();
        }
        public async Task<List<Report_ProductSale>> GetSingleCookReportForTakeoutAsync(int cookId, DateTime start, DateTime end)
        {
            var query = from relative in Context.CookProductRelatives
                        join product in Context.OrderProducts on relative.ProductId equals product.ProductId into joinProduct
                        from product in joinProduct.DefaultIfEmpty()
                        join order in Context.Orders on product.OrderId equals order.ID
                        where relative.StaffId == cookId && (order.Status & OrderStatus.Valid) > 0 && product.CreateTime >= start && product.CreateTime < end
                        group product by new { product.ProductId, product.Name } into g
                        select new Report_ProductSale
                        {
                            Id = g.Key.ProductId.Value,
                            Name = g.Key.Name,
                            Count = g.Sum(a => a.Quantity ?? 0),
                            Amount = g.Sum(a => a.Price ?? 0)
                        };
            return await query.OrderByDescending(a => a.Amount).ToListAsync();
        }

        public async Task<List<Report_ProductSale>> GetBoothsReportAsync(int businessId, DateTime start, DateTime end)
        {
            var booths = await Context.StoreBooths.Where(a => a.BusinessId == businessId)
                .Select(a => new Report_ProductSale { Id = a.ID, Name = a.Name }).ToListAsync();
            if (booths.Count == 0) return null;
            var ids = booths.Select(a => a.Id).ToList();

            var productQuery = from relative in Context.BoothProductRelatives
                               join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId
                               where ids.Contains(relative.StoreBoothId) && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                               select new { relative.StoreBoothId, product.Quantity, product.Amount };

            var products = await productQuery.ToListAsync();

            booths.ForEach(a =>
            {
                var p = products.Where(b => a.Id == b.StoreBoothId).ToList();
                if (p.Count == 0) return;
                a.Count = p.Sum(c => c.Quantity);
                a.Amount = p.Sum(c => c.Amount);
            });
            return booths;
        }
        public async Task<List<Report_ProductSale>> GetBoothsReportForTakeoutAsync(int businessId, DateTime start, DateTime end)
        {
            var booths = await Context.StoreBooths.Where(a => a.BusinessId == businessId)
                .Select(a => new Report_ProductSale { Id = a.ID, Name = a.Name }).ToListAsync();
            if (booths.Count == 0) return null;
            var ids = booths.Select(a => a.Id).ToList();

            var productQuery = from relative in Context.BoothProductRelatives
                               join product in Context.OrderProducts on relative.ProductId equals product.ProductId
                               join order in Context.Orders on product.OrderId equals order.ID
                               where ids.Contains(relative.StoreBoothId) && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                               select new { relative.StoreBoothId, product.Quantity, product.Price };

            var products = await productQuery.ToListAsync();

            booths.ForEach(a =>
            {
                var p = products.Where(b => a.Id == b.StoreBoothId).ToList();
                if (p.Count == 0) return;
                a.Count = p.Sum(c => c.Quantity ?? 0);
                a.Amount = p.Sum(c => c.Price ?? 0);
            });
            return booths;
        }

        public async Task<List<Report_ProductSale>> GetSingleBoothReportAsync(int boothId, DateTime start, DateTime end)
        {
            var query = from relative in Context.BoothProductRelatives
                        join product in Context.TangOrderProducts on relative.ProductId equals product.ProductId into joinProduct
                        from product in joinProduct.DefaultIfEmpty()
                        where relative.StoreBoothId == boothId && product.ProductStatus != TangOrderProductStatus.Return && product.CreateTime >= start && product.CreateTime < end
                        group product by new { product.ProductId, product.Name } into g
                        select new Report_ProductSale
                        {
                            Id = g.Key.ProductId,
                            Name = g.Key.Name,
                            Count = g.Sum(a => a.Quantity),
                            Amount = g.Sum(a => a.Amount)
                        };
            return await query.OrderByDescending(a => a.Amount).ToListAsync();
        }
        public async Task<List<Report_ProductSale>> GetSingleBoothReportForTakeoutAsync(int boothId, DateTime start, DateTime end)
        {
            var query = from relative in Context.BoothProductRelatives
                        join product in Context.OrderProducts on relative.ProductId equals product.ProductId into joinProduct
                        from product in joinProduct.DefaultIfEmpty()
                        join order in Context.Orders on product.OrderId equals order.ID
                        where relative.StoreBoothId == boothId && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                        group product by new { product.ProductId, product.Name } into g
                        select new Report_ProductSale
                        {
                            Id = g.Key.ProductId.Value,
                            Name = g.Key.Name,
                            Count = g.Sum(a => a.Quantity ?? 0),
                            Amount = g.Sum(a => a.Price ?? 0)
                        };
            return await query.OrderByDescending(a => a.Amount).ToListAsync();
        }

        public async Task<List<Report_ProductRanking>> GetProductsDataAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from orderProduct in Context.TangOrderProducts
                        join product in Context.Products on orderProduct.ProductId equals product.ID
                        where product.BusinessId == businessId && orderProduct.CreateTime >= start && orderProduct.CreateTime < end
                        group orderProduct by new { product.ID, product.Name } into g
                        select new Report_ProductRanking
                        {
                            Id = g.Key.ID,
                            Name = g.Key.Name,
                            Quantity = g.Sum(a => a.Quantity),
                            Amount = g.Sum(a => a.OriginalPrice * a.Quantity),
                            SaleAmount = g.Where(a => a.ProductStatus != TangOrderProductStatus.Return).Sum(a => a.OriginalPrice * a.Quantity),
                            ActualAmount = g.Where(a => a.ProductStatus != TangOrderProductStatus.Return).Sum(a => a.Amount),
                            CancelQuantity = g.Where(a => a.ProductStatus == TangOrderProductStatus.Return).Sum(a => a.Quantity),
                            CancelAmount = g.Where(a => a.ProductStatus == TangOrderProductStatus.Return).Sum(a => a.Amount),
                            DiscountQuantity = g.Where(a => a.Discount < 10).Sum(a => a.Quantity),
                            DiscountAmount = g.Where(a => a.Discount < 10).Sum(a => a.Quantity * (a.OriginalPrice - a.Price)),
                            DiscountedAmount = g.Where(a => a.Discount < 10).Sum(a => a.Amount),

                        };
            var list = await query.ToListAsync();
            list.ForEach(item =>
            {
                item.CancelSaleAmount = item.Amount - item.SaleAmount;
                item.SaleQuantity = item.ActualQuantity = item.Quantity - item.CancelQuantity;
            });
            list.Sort((a, b) => (int)(b.ActualAmount - a.ActualAmount));
            return list;
        }
        public async Task<List<Report_ProductRanking>> GetProductsDataForTakeoutAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from orderProduct in Context.OrderProducts
                        join order in Context.Orders on orderProduct.OrderId equals order.ID
                        where order.BusinessId == businessId && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                        group orderProduct by new { orderProduct.ProductId, orderProduct.Name } into g
                        select new Report_ProductRanking
                        {
                            Id = g.Key.ProductId.Value,
                            Name = g.Key.Name,
                            Quantity = g.Sum(a => a.Quantity ?? 0),
                            Amount = g.Sum(a => a.OldPrice ?? 0),
                            ActualAmount = g.Sum(a => a.Price ?? 0),
                            DiscountQuantity = g.Sum(a => a.DiscountProductQuantity ?? 0),
                            DiscountAmount = g.Where(a => a.DiscountProductQuantity > 0).Sum(a => (a.OldPrice ?? 0) - (a.Price ?? 0)),
                            DiscountedAmount = g.Where(a => a.DiscountProductQuantity > 0).Sum(a => a.Price ?? 0)
                        };
            var list = await query.ToListAsync();
            list.ForEach(item =>
            {
                item.SaleAmount = item.Amount;
                item.SaleQuantity = item.ActualQuantity = item.Quantity;
            });
            list.Sort((a, b) => (int)(b.ActualAmount - a.ActualAmount));
            return list;
        }

        public async Task<List<Report_Setmeal>> GetSetmealDataAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from orderProduct in Context.TangOrderProducts
                        join relative in Context.ProductRelatives on orderProduct.ProductId equals relative.SetMealId
                        join product in Context.Products on relative.ProductId equals product.ID
                        where orderProduct.CreateTime >= start && orderProduct.CreateTime < end && orderProduct.ProductStatus != TangOrderProductStatus.Return && orderProduct.Feature == ProductFeature.SetMeal
                        select new { product.ID, product.Name, SetmealName = orderProduct.Name, SetmealQuantity = orderProduct.Quantity };

            var list = await query.ToListAsync();
            var result = list.GroupBy(a => new { a.ID, a.Name })
                .Select(a => new Report_Setmeal
                {
                    Id = a.Key.ID,
                    Name = a.Key.Name,
                    Quantity = a.Sum(b => b.SetmealQuantity),
                    SetMeals = a.GroupBy(b => b.SetmealName).Select(b => new Tuple<string, double>(b.Key, b.Sum(c => c.SetmealQuantity))).ToList()
                })
                .ToList();
            result.Sort((a, b) => (int)(b.Quantity - a.Quantity));
            return result;
        }
        public async Task<List<Report_Setmeal>> GetSetmealDataForTakeoutAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from order in Context.Orders
                        join orderProduct in Context.OrderProducts on order.ID equals orderProduct.OrderId
                        join relative in Context.ProductRelatives on orderProduct.ProductId equals relative.SetMealId
                        join product in Context.Products on relative.ProductId equals product.ID
                        where order.BusinessId == businessId && order.CreateTime >= start && order.CreateTime < end && (order.Status & OrderStatus.Valid) > 0 && orderProduct.Feature == ProductFeature.SetMeal
                        select new { product.ID, product.Name, SetmealName = orderProduct.Name, SetmealQuantity = orderProduct.Quantity };
            var list = await query.ToListAsync();
            var result = list.GroupBy(a => new { a.ID, a.Name })
                .Select(a => new Report_Setmeal
                {
                    Id = a.Key.ID,
                    Name = a.Key.Name,
                    Quantity = a.Sum(b => b.SetmealQuantity ?? 0),
                    SetMeals = a.GroupBy(b => b.SetmealName).Select(b => new Tuple<string, double>(b.Key, b.Sum(c => c.SetmealQuantity ?? 0))).ToList()
                })
                .ToList();
            result.Sort((a, b) => (int)(b.Quantity - a.Quantity));
            return result;
        }

        public async Task<List<Report_Benefit>> GetBenefitDataAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from order in Context.TangOrders
                        where order.BusinessId == businessId && (order.OrderStatus & TangOrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                        group order by order.PaymentRemark into g
                        select new Report_Benefit
                        {
                            Name = g.Key,
                            Amount = g.Sum(a => a.PreferentialAmount),
                            OrderAmount = g.Sum(a => a.Amount),
                            Quantity = g.Count()
                        };
            return await query.ToListAsync();
        }

        public async Task<object> GetSingleBenetifDataAsync(int businessId, string name, DateTime start, DateTime end)
        {
            var query = from order in Context.TangOrders
                        where order.BusinessId == businessId && (order.OrderStatus & TangOrderStatus.Valid) > 0 && order.CreateTime >= start && order.CreateTime < end
                        select new
                        {
                            order.ID, order.Code, order.PayTime, order.Amount, order.ActualAmount, order.PaymentRemark
                        };
            if (string.IsNullOrEmpty(name))
            {
                query = query.Where(a => a.PaymentRemark == "" || a.PaymentRemark == null);
            }
            else
            {
                query = query.Where(a => a.PaymentRemark == name);
            }
            return await query.ToListAsync();
        }

        public async Task<List<Report_Payment>> GetPaymentDataAsync(int businessId, DateTime start, DateTime end)
        {
            var query = from order in Context.TangOrders
                        join payment in Context.TangOrderPayments on order.ID equals payment.TangOrderId
                        where order.BusinessId == businessId && (order.OrderStatus & TangOrderStatus.Valid) > 0 && order.PayTime >= start && order.PayTime < end
                        group payment by new { payment.PaymentTypeId, payment.Name } into g
                        select new Report_Payment
                        {
                            Id = g.Key.PaymentTypeId,
                            Name = g.Key.Name,
                            Quantity = g.Count(),
                            Amount = g.Sum(a => a.Amount)
                        };
            return await query.ToListAsync();
        }

        public async Task<object> GetSinglePaymentDataAsync(int id, DateTime start, DateTime end)
        {
            var query = from order in Context.TangOrders
                        join payment in Context.TangOrderPayments on order.ID equals payment.TangOrderId
                        where payment.PaymentTypeId == id && (order.OrderStatus & TangOrderStatus.Valid) > 0 && order.PayTime >= start && order.PayTime < end
                        group order by new { order.ID, order.Code, order.PayTime, order.Amount, order.ActualAmount } into g
                        select g.Key;

            return await query.ToListAsync();
        }

    }
}
