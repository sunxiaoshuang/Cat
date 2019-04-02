using System;
using System.Collections.Generic;
using System.Data;
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
using JdCat.Cat.Repository.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JdCat.Cat.Repository
{
    public class BusinessRepository : BaseRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(CatDbContext context) : base(context)
        {

        }

        public Business GetBusiness(Expression<Func<Business, bool>> expression)
        {
            return Context.Businesses.AsNoTracking().SingleOrDefault(expression);
        }
        public Business GetBusinessByStoreId(string code)
        {
            var p = code.ToUpper();
            return Context.Businesses.SingleOrDefault(a => a.StoreId == p);
        }
        public bool ExistForCode(string code)
        {
            return Context.Businesses.Count(a => a.Code == code) > 0;
        }
        public bool SaveBase(Business business)
        {
            //var entity = new Business { ID = business.ID, Lng = -1, Lat = -1, MinAmount = -1, Range = -1 };
            var entity = Get(business.ID);
            //Context.Attach(entity);
            entity.Name = business.Name;
            entity.Email = business.Email;
            entity.Address = business.Address;
            entity.Contact = business.Contact;
            entity.Mobile = business.Mobile;
            entity.FreightMode = business.FreightMode;
            entity.Freight = business.Freight;
            entity.Description = business.Description;
            entity.Range = business.Range;
            entity.LogoSrc = business.LogoSrc;
            entity.BusinessLicense = business.BusinessLicense;
            entity.BusinessLicenseImage = business.BusinessLicenseImage;
            entity.SpecialImage = business.SpecialImage;
            entity.Lng = business.Lng;
            entity.Lat = business.Lat;
            entity.BusinessStartTime = business.BusinessStartTime;
            entity.BusinessEndTime = business.BusinessEndTime;
            entity.BusinessStartTime2 = business.BusinessStartTime2;
            entity.BusinessEndTime2 = business.BusinessEndTime2;
            entity.BusinessStartTime3 = business.BusinessStartTime3;
            entity.BusinessEndTime3 = business.BusinessEndTime3;
            entity.MinAmount = business.MinAmount;
            entity.ServiceProvider = business.ServiceProvider;
            entity.Province = business.Province;
            entity.City = business.City;
            entity.Area = business.Area;
            return Context.SaveChanges() > 0;
        }
        public bool ChangeAutoReceipt(Business business, bool state)
        {
            var entity = new Business { ID = business.ID, IsAutoReceipt = business.IsAutoReceipt };
            Context.Attach(entity);
            entity.IsAutoReceipt = state;
            return Context.SaveChanges() > 0;
        }
        public bool ChangeClose(Business business, bool state)
        {
            var entity = new Business { ID = business.ID, IsClose = business.IsClose };
            Context.Attach(entity);
            entity.IsClose = state;
            return Context.SaveChanges() > 0;
        }

        public bool SaveSmall(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.AppId = business.AppId;
            entity.Secret = business.Secret;
            entity.MchId = business.MchId;
            entity.MchKey = business.MchKey;
            entity.TemplateNotifyId = business.TemplateNotifyId;
            entity.AppQrCode = business.AppQrCode;
            return Context.SaveChanges() > 0;
        }

        public bool SaveDada(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            //entity.DadaAppKey = business.DadaAppKey;
            //entity.DadaAppSecret = business.DadaAppSecret;
            entity.DadaSourceId = business.DadaSourceId;
            entity.DadaShopNo = business.DadaShopNo;
            entity.CityCode = business.CityCode;
            entity.CityName = business.CityName;
            return Context.SaveChanges() > 0;
        }

        public async Task<string> UploadAsync(string url, int businessId, string source)
        {

            using (var hc = new HttpClient())
            {
                var param = JsonConvert.SerializeObject(new
                {
                    BusinessId = businessId,
                    Name = Guid.NewGuid().ToString().ToLower(),
                    Image400 = source
                });
                var httpcontent = new StringContent(param);
                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var msg = await hc.PostAsync(url, httpcontent);
                return JsonConvert.DeserializeObject<string>(await msg.Content.ReadAsStringAsync());
            }
        }

        public bool SaveFeyin(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.FeyinApiKey = business.FeyinApiKey;
            entity.FeyinMemberCode = business.FeyinMemberCode;
            return Context.SaveChanges() > 0;
        }

        public IEnumerable<FeyinDevice> GetPrinters(int businessId)
        {
            return Context.FeyinDevices.Where(a => a.BusinessId == businessId).ToList();
        }
        public async Task<bool> BindPrintDeviceAsync(Business business, FeyinDevice device)
        {
            device.BusinessId = business.ID;
            if (device.Type == PrinterType.Feyin)
            {
                device.MemberCode = business.FeyinMemberCode;
                device.ApiKey = business.FeyinApiKey;
            }
            else if (device.Type == PrinterType.Feie)
            {
                var ret = await FeieHelper.GetHelper().AddPrintAsync(device);
                if (ret.ret != 0)
                {
                    return false;
                }
            }
            else if (device.Type == PrinterType.Waimaiguanjia)
            {
                var ret = await WmgjHelper.GetHelper().AddPrintAsync(device);
                if (ret.data == null || string.IsNullOrEmpty(ret.data.token))
                {
                    return false;
                }
                device.Remark = ret.data.token;
            }

            Context.FeyinDevices.Add(device);
            return Context.SaveChanges() > 0;
        }

        public async Task<JsonData> UnbindPrintDeviceAsync(int id)
        {
            var result = new JsonData();
            var device = Context.FeyinDevices.SingleOrDefault(a => a.ID == id);
            if (device == null)
            {
                result.Msg = "设备可以已经被删除，请刷新后重试";
                return result;
            }
            if (device.Type == PrinterType.Waimaiguanjia)
            {
                await WmgjHelper.GetHelper().DelPrinterAsync(device);
            }

            Context.FeyinDevices.Remove(device);
            result.Success = Context.SaveChanges() > 0;
            if (!result.Success)
            {
                result.Msg = "解除绑定失败，请刷新后重试";
            }
            result.Msg = "解除绑定成功";
            return result;
        }

        public bool UpdatePassword(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.Password = business.Password;
            return Context.SaveChanges() > 0;
        }

        public bool SetDefaultPrinter(Business business, int id)
        {
            var printers = Context.FeyinDevices.Where(a => a.BusinessId == business.ID).ToList();
            foreach (var item in printers)
            {
                item.IsDefault = item.ID == id;
            }

            return Context.SaveChanges() > 0;
        }

        public List<Report_Order> GetOrderTotal(Business business, DateTime startTime, DateTime endTime)
        {
            var id = business.ID;

            var query1 = from order in Context.Orders
                         where order.BusinessId.Value == id && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= startTime && order.CreateTime < endTime
                         select new { CreateTime = order.CreateTime.Value.ToString("yyyy-MM-dd"), order.Price };

            var query2 = from order in query1
                         group order by order.CreateTime into g
                         select new Report_Order { CreateTime = g.Key, Price = g.Sum(a => a.Price.Value), Quantity = g.Count() };

            return query2.ToList();


            //return ExecuteReader<Report_Order>($@"select CreateTime, SUM(Price) Price, COUNT(*) Quantity from 
            //    (select Price, CONVERT(varchar(10), CreateTime, 120) as CreateTime from dbo.[Order] where BusinessId={business.ID} and Status & {(int)OrderStatus.Valid} > 0 and CreateTime between '{startTime:yyyy-MM-dd}' and '{endTime:yyyy-MM-dd}')t1
            //group by CreateTime");
        }

        public List<Report_Product> GetProductTop10(Business business, DateTime date)
        {
            var time = date.ToString("yyyy-MM-dd");
            var query = from obj in Context.OrderProducts
                        join product in Context.Products on obj.ProductId equals product.ID
                        join order in Context.Orders on obj.OrderId equals order.ID
                        where product.BusinessId == business.ID && (order.Status & OrderStatus.Invalid) == 0 && order.CreateTime.Value.ToString("yyyy-MM-dd") == time
                        group obj by new { obj.ProductId, obj.Name } into g
                        orderby g.Sum(a => a.Quantity ?? 0) descending
                        select new Report_Product { Name = g.Key.Name, Quantity = g.Sum(a => a.Quantity ?? 0) };

            return query.Take(10).ToList();


            //        return ExecuteReader<Report_Product>($@"
            //        select top 10 b.ID, b.Name, sum(a.Quantity) Quantity from dbo.[OrderProduct] a
            //         inner join dbo.[Product] b on a.ProductId = b.ID
            //inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            //        where b.BusinessId = {business.ID} and convert(varchar(10), c.CreateTime, 120) = '{date:yyyy-MM-dd}'
            //        group by b.ID, b.Name
            //        order by Quantity desc
            //        ");
        }
        public List<Report_ProductPrice> GetProductPriceTop10(Business business, DateTime date)
        {
            var time = date.ToString("yyyy-MM-dd");
            var query = from obj in Context.OrderProducts
                        join product in Context.Products on obj.ProductId equals product.ID
                        join order in Context.Orders on obj.OrderId equals order.ID
                        where product.BusinessId == business.ID && (order.Status & OrderStatus.Invalid) == 0 && order.CreateTime.Value.ToString("yyyy-MM-dd") == time
                        group obj by new { obj.ProductId, obj.Name } into g
                        orderby g.Sum(a => a.Price ?? 0) descending
                        select new Report_ProductPrice { Name = g.Key.Name, Amount = g.Sum(a => a.Price ?? 0) };
            return query.Take(10).ToList();

            //        return ExecuteReader<Report_ProductPrice>($@"
            //        select top 10 b.ID, b.Name, sum(a.Price) Amount from dbo.[OrderProduct] a
            //         inner join dbo.[Product] b on a.ProductId = b.ID
            //inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            //        where b.BusinessId = {business.ID} and convert(varchar(10), c.CreateTime, 120) = '{date:yyyy-MM-dd}'
            //        group by b.ID, b.Name
            //        order by Amount desc
            //        ");
        }

        public List<Report_SaleStatistics> GetSaleStatistics(Business business, DateTime start, DateTime end)
        {
            var startTime = new DateTime(start.Year, start.Month, start.Day); ;
            var endTime = new DateTime(end.Year, end.Month, end.Day);
            endTime = endTime.AddDays(1);

            var query1 = from order in Context.Orders
                         where order.BusinessId == business.ID && (order.Status & OrderStatus.Valid) > 0 && order.CreateTime >= startTime && order.CreateTime < endTime
                         select new { order.Price, order.PackagePrice, order.Freight, CreateTime = order.CreateTime.Value.ToString("yyyy-MM-dd") };

            var query2 = from order in query1
                         group order by order.CreateTime into g
                         select new Report_SaleStatistics
                         {
                             Date = g.Key,
                             Total = g.Sum(a => a.Price ?? 0),
                             PackageAmount = g.Sum(a => a.PackagePrice ?? 0),
                             FreightAmount = g.Sum(a => a.Freight ?? 0),
                             Quantity = g.Count()
                         };
            return query2.OrderByDescending(a => a.Date).ToList();


            //return ExecuteReader<Report_SaleStatistics>($@"
            //    select CreateTime AS [Date], sum(Price) Total, sum(PackagePrice) PackageAmount, sum(Freight) FreightAmount, count(CreateTime) Quantity from (
            //     select Price, PackagePrice, Freight, convert(VARCHAR(10), CreateTime, 120) CreateTime from dbo.[Order] where 
            //     [BusinessId] = {business.ID} and 
            //     [Status] & {(int)OrderStatus.Valid} > 0 and 
            //     [CreateTime] between '{start:yyyy-MM-dd}' and '{end.AddDays(1):yyyy-MM-dd}' 
            //    )t
            //    group by t.CreateTime
            //    order by CreateTime desc
            //");
        }

        public SaleFullReduce GetFullReduceById(int id)
        {
            return Context.SaleFullReduces.Single(a => a.ID == id);
        }

        public JsonData CreateFullReduce(SaleFullReduce entity)
        {
            var result = ValidateFullReduce(entity);
            if (!result.Success)
            {
                return result;
            }
            entity.ModifyTime = DateTime.Now;
            Context.SaleFullReduces.Add(entity);
            result.Success = Context.SaveChanges() > 0;
            if (!result.Success)
            {
                result.Msg = "创建失败，请刷新后重试";
                return result;
            }
            result.Msg = "创建成功";
            return result;
        }
        public JsonData UpdateFullReduce(SaleFullReduce entity)
        {
            var result = ValidateFullReduce(entity);
            if (!result.Success)
            {
                return result;
            }
            var obj = new SaleFullReduce { ID = entity.ID, IsForeverValid = !entity.IsForeverValid };
            Context.Attach(obj);
            obj.ModifyTime = DateTime.Now;
            obj.IsForeverValid = entity.IsForeverValid;
            //obj.MinPrice = entity.MinPrice;
            obj.StartDate = entity.StartDate;
            obj.EndDate = entity.EndDate;
            obj.Name = entity.Name;
            //obj.ReduceMoney = entity.ReduceMoney;

            result.Success = Context.SaveChanges() > 0;
            if (!result.Success)
            {
                result.Msg = "修改失败，请刷新后重试";
                return result;
            }
            result.Msg = "修改成功";
            return result;
        }

        public IEnumerable<SaleFullReduce> GetFullReduce(Business business, bool tracking = true)
        {
            if (!tracking) return Context.SaleFullReduces.AsNoTracking().Where(a => a.BusinessId == business.ID && !a.IsDelete).OrderBy(a => a.ReduceMoney);
            return Context.SaleFullReduces.Where(a => a.BusinessId == business.ID && !a.IsDelete).OrderBy(a => a.ReduceMoney);
        }

        public JsonData DeleteFullReduce(int id)
        {
            var entity = new SaleFullReduce { ID = id };
            Context.Attach(entity);
            entity.IsDelete = true;
            var result = new JsonData { Success = Context.SaveChanges() > 0 };
            if (result.Success)
            {
                result.Msg = "删除成功";
            }
            else
            {
                result.Msg = "删除失败，请刷新后重试";
            }
            return result;
        }

        public JsonData CreateCoupon(SaleCoupon saleCoupon)
        {
            var result = ValidateCoupon(saleCoupon);
            if (!result.Success) return result;
            Context.SaleCoupons.Add(saleCoupon);
            result.Success = Context.SaveChanges() > 0;
            if (!result.Success)
            {
                result.Msg = "创建失败，请刷新后重试";
                return result;
            }
            result.Msg = "创建成功";
            return result;
        }
        public SaleCoupon GetCouponById(int id)
        {
            return Context.SaleCoupons.Single(a => a.ID == id);
        }
        public IEnumerable<SaleCoupon> GetCoupon(Business business, bool tracking = false)
        {
            if (!tracking) return Context.SaleCoupons.AsNoTracking().Where(a => a.BusinessId == business.ID && !a.IsDelete);
            return Context.SaleCoupons.Where(a => a.BusinessId == business.ID && !a.IsDelete);
        }

        public List<SaleCoupon> GetCouponValid(Business business, bool tracking = false)
        {
            var list = GetCoupon(business, tracking).Where(a => a.IsActive).ToList();
            return list.Where(a => a.Status == CouponStatus.Up).ToList();
        }
        public JsonData DeleteCoupon(int id)
        {
            var entity = new SaleCoupon { ID = id };
            Context.Attach(entity);
            entity.IsDelete = true;
            var result = new JsonData { Success = Context.SaveChanges() > 0 };
            if (result.Success)
            {
                result.Msg = "删除成功";
            }
            else
            {
                result.Msg = "删除失败，请刷新后重试";
            }
            return result;
        }
        public JsonData DownCoupon(int id)
        {
            var entity = new SaleCoupon { ID = id, IsActive = true };
            Context.Attach(entity);
            entity.IsActive = false;
            var result = new JsonData { Success = Context.SaveChanges() > 0 };
            if (result.Success)
            {
                result.Msg = "下架成功";
            }
            else
            {
                result.Msg = "下架失败，请刷新后重试";
            }
            return result;
        }

        public IEnumerable<SaleProductDiscount> GetDiscounts(Business business)
        {
            return Context.SaleProductDiscount.Where(a => a.BusinessId == business.ID && a.Status != ActivityStatus.Delete);
        }
        public JsonData CreateDiscount(SaleProductDiscount discount)
        {

            return null;
        }
        public List<SaleProductDiscount> CreateDiscount(List<SaleProductDiscount> list)
        {
            list.ForEach(discount =>
            {
                discount.Cycle = WeekdayState.All;
                discount.StartTime1 = "00:00";
                discount.EndTime1 = "23:59";
                discount.SettingType = "1";
                discount.Status = ActivityStatus.Init;
                discount.UpperLimit = 1;
                Context.SaleProductDiscount.Add(discount);

            });
            Commit();
            return list;
        }
        public JsonData DeleteDiscount(int id)
        {
            var result = new JsonData();
            var discount = new SaleProductDiscount { ID = id };
            Context.Attach(discount);
            discount.Status = ActivityStatus.Delete;
            result.Success = Context.SaveChanges() > 0;
            result.Msg = "删除成功";
            return result;
        }
        public JsonData UpdateDiscount(SaleProductDiscount discount)
        {
            var result = new JsonData();
            var entity = Context.SaleProductDiscount.Single(a => a.ID == discount.ID);
            entity.Status = ActivityStatus.Active;
            entity.StartDate = discount.StartDate;
            entity.EndDate = discount.EndDate;
            entity.Discount = discount.Discount;
            entity.Price = discount.Price;
            entity.Cycle = discount.Cycle;
            entity.StartTime1 = discount.StartTime1;
            entity.EndTime1 = discount.EndTime1;
            entity.StartTime2 = discount.StartTime2;
            entity.EndTime2 = discount.EndTime2;
            entity.StartTime3 = discount.StartTime3;
            entity.EndTime3 = discount.EndTime3;
            entity.SettingType = discount.SettingType;
            entity.UpperLimit = discount.UpperLimit;
            result.Success = Context.SaveChanges() > 0;
            result.Data = entity;
            return result;
        }

        public Business Login(string username, string password)
        {
            return Context.Businesses.SingleOrDefault(a => a.Code == username && a.Password == password);
        }

        public List<ClientPrinter> GetClientPrinters(int id)
        {
            return Context.ClientPrinters.Where(a => a.BusinessId == id).ToList();
        }

        public void SavePrinters(IEnumerable<ClientPrinter> printers)
        {
            foreach (var item in printers)
            {
                Context.Add(item);
            }
            Context.SaveChanges();
        }
        public void SavePrinter(ClientPrinter printer)
        {
            if (printer.ID > 0)
            {
                Context.Update(printer);
            }
            else
            {
                Context.Add(printer);
            }
            Context.SaveChanges();
        }
        public void PutPrinterProducts(int id, string ids)
        {
            var printer = Context.ClientPrinters.FirstOrDefault(a => a.ID == id);
            if (printer == null) return;
            if (printer.FoodIds == ids) return;
            printer.FoodIds = ids;
            Context.SaveChanges();
        }
        public List<DeskType> GetDeskTypes(int id)
        {
            var types = Context.DeskTypes
                .Include(a => a.Desks)
                .AsNoTracking()
                .Where(a => a.BusinessId == id && !a.IsDelete)
                .ToList();
            foreach (var type in types)
            {
                if (type.Desks == null) continue;
                var deleteList = type.Desks.Where(a => a.IsDelete).ToList();
                foreach (var desk in deleteList)
                {
                    type.Desks.Remove(desk);
                }
            }
            return types;
        }
        public DeskType SaveDeskType(DeskType type)
        {
            Context.DeskTypes.Add(type);
            Context.SaveChanges();
            return type;
        }
        public DeskType UpdateDeskType(DeskType type)
        {
            var entity = Context.DeskTypes.FirstOrDefault(a => a.ID == type.ID);
            if (entity == null) return null;
            entity.Name = type.Name;
            entity.Sort = type.Sort;
            Context.SaveChanges();
            return entity;
        }
        public void DeleteDeskType(int id)
        {
            var entity = Context.DeskTypes.FirstOrDefault(a => a.ID == id);
            if (entity == null) return;
            entity.IsDelete = true;
            Context.SaveChanges();
        }
        public Desk SaveDesk(Desk desk)
        {
            Context.Desks.Add(desk);
            Context.SaveChanges();
            return desk;
        }
        public Desk UpdateDesk(Desk desk)
        {
            var entity = Context.Desks.FirstOrDefault(a => a.ID == desk.ID);
            if (entity == null) return null;
            entity.Name = desk.Name;
            entity.Quantity = desk.Quantity;
            Context.SaveChanges();
            return entity;
        }
        public void DeleteDesk(int id)
        {
            var entity = Context.Desks.FirstOrDefault(a => a.ID == id);
            if (entity == null) return;
            entity.IsDelete = true;
            Context.SaveChanges();
        }


        /// <summary>
        /// 验证满减活动输入是否正确
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private JsonData ValidateFullReduce(SaleFullReduce entity)
        {
            var result = new JsonData();
            if (!entity.IsForeverValid)
            {
                if (entity.StartDate > entity.EndDate)
                {
                    result.Msg = "活动开始时间不能大于活动结束时间";
                    return result;
                }
            }
            if (entity.MinPrice <= 0)
            {
                result.Msg = "最低消费金额必须大于零";
                return result;
            }
            if (entity.ReduceMoney <= 0)
            {
                result.Msg = "减少金额必须大于零";
                return result;
            }
            result.Success = true;
            return result;
        }
        /// <summary>
        /// 验证优惠券信息是否正确
        /// </summary>
        /// <returns></returns>
        private JsonData ValidateCoupon(SaleCoupon coupon)
        {
            var result = new JsonData();
            if (coupon.Value <= 0)
            {
                result.Msg = "代金面额必须大于零";
                return result;
            }
            if (coupon.ValidDay < 0)
            {
                if (coupon.StartDate == null || coupon.EndDate == null)
                {
                    result.Msg = "优惠券有效期输入不正确";
                    return result;
                }
            }
            if (coupon.MinConsume < 0) coupon.MinConsume = -1;
            if (coupon.Quantity < 0)
            {
                coupon.Quantity = -1;
            }
            else
            {
                coupon.Stock = coupon.Quantity;
            }
            result.Success = true;
            return result;
        }

        public List<WxListenUser> GetWxListenUser(int businessId)
        {
            return Context.WxListenUsers.AsNoTracking().Where(a => a.BusinessId == businessId).ToList();
        }
        public void BindWxListen(WxListenUser user)
        {
            Context.WxListenUsers.Add(user);
            Context.SaveChanges();
        }
        public void SaveWxQrcode(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.WxQrListenPath = business.WxQrListenPath;
            Context.SaveChanges();
        }
        public void RemoveWxListenUser(int id)
        {
            Context.WxListenUsers.Remove(new WxListenUser { ID = id });
            Context.SaveChanges();
        }
        public List<BusinessFreight> GetFreights(int businessId)
        {
            return Context.BusinessFreights.Where(a => a.BusinessId == businessId).OrderBy(a => a.MaxDistance).ToList();
        }
        public BusinessFreight CreateFreight(BusinessFreight freight)
        {
            Context.BusinessFreights.Add(freight);
            Context.SaveChanges();
            return freight;
        }
        public BusinessFreight UpdateFreight(BusinessFreight freight)
        {
            var entity = new BusinessFreight { ID = freight.ID, Amount = -1 };
            Context.Attach(entity);
            entity.MaxDistance = freight.MaxDistance;
            entity.Amount = freight.Amount;
            entity.ModifyTime = DateTime.Now;
            Context.SaveChanges();
            return entity;
        }
        public bool RemoveFreight(int id)
        {
            Context.BusinessFreights.Remove(new BusinessFreight { ID = id });
            return Context.SaveChanges() > 0;
        }

        public string GetNextStoreNumber()
        {
            return ExecuteScalar("SELECT CONCAT('JD', fn_right_padding(NEXT_VAL('StoreNumbers'), 6))") + "";
        }

        public List<OrderComment> GetComments(int id, PagingQuery paging)
        {
            var query = Context.OrderComments.Where(a => a.IsShow && a.BusinessId == id)
                .OrderByDescending(a => a.CreateTime)
                .Skip(paging.Skip)
                .Take(paging.PageSize);
            return query.ToList();
        }

        public OpenAuthInfo AddAuthInfo(WxAuthInfo info, Business business)
        {
            var entity = Context.OpenAuthInfos.FirstOrDefault(a => a.BusinessId == business.ID);
            if (entity == null)
            {
                // 新授权
                entity = new OpenAuthInfo
                {
                    AppId = info.authorization_info.authorizer_appid,
                    BusinessId = business.ID,
                    CreateTime = DateTime.Now,
                    RefreshToken = info.authorization_info.authorizer_refresh_token
                };
                Context.OpenAuthInfos.Add(entity);
            }
            else
            {
                // 修改之前的授权
                entity.AppId = info.authorization_info.authorizer_appid;
                entity.RefreshToken = info.authorization_info.authorizer_refresh_token;
                entity.ModifyTime = DateTime.Now;
            }
            Context.SaveChanges();
            return entity;
        }
        public OpenAuthInfo GetAuthInfo(int businessId)
        {
            return Context.OpenAuthInfos.SingleOrDefault(a => a.BusinessId == businessId);
        }

        public List<Business> GetStores(int chainId, PagingQuery query)
        {
            var list = Context.Businesses.Where(a => a.ParentId == chainId);
            query.RecordCount = list.Count();
            if (query.RecordCount == 0) return new List<Business>();
            return list.Skip(query.Skip).Take(query.PageSize).ToList();
        }

        public Business BindStore(Business chain, Business store)
        {
            Context.Attach(store);
            store.ParentId = chain.ID;
            store.AppId = chain.AppId;
            store.Secret = chain.Secret;
            store.TemplateNotifyId = chain.TemplateNotifyId;
            store.AppQrCode = chain.AppQrCode;
            Context.SaveChanges();
            return store;
        }

        public bool UnBindStore(Business chain, Business store)
        {
            Context.Attach(store);
            store.ParentId = null;
            return Context.SaveChanges() > 0;
        }

        public Business GetNearestStore(int chainId, double lat, double lng)
        {
            var business = Context.Businesses.FromSql($@"select *, fn_geo(Lat, Lng, {lat}, {lng}) as Distance from Business a
                        where ParentId={chainId} and IsClose=0 and Category=0
	                    order by Distance asc 
	                    limit 1").ToList();
            if (business == null || business.Count == 0) return null;
            return business[0];
        }

        public List<Business> GetNearbyStore(int chainId, string city, double lat, double lng, string key = null)
        {
            if (key != null && key.Contains("'")) throw new ArgumentException("参数中不能包含单引号");
            var condition = $" where ParentId={chainId} and City='{city}' and Category=0";
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(key.Trim()))
            {
                condition += $" and (locate('{key}', Name) > 0 or locate('{key}', Address) > 0)";
            }
            var sql = $@"select *, fn_geo(Lat, Lng, {lat}, {lng}) as Distance from Business a
                        {condition}
                        order by Distance asc
                        limit 7";
            return Context.Businesses.FromSql(sql).ToList();
            //return ExecuteReader<Business>(sql);
        }

        public string ResetPwd(int id)
        {
            var password = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6).ToLower();
            var business = new Business { ID = id };
            Context.Attach(business);
            business.Password = UtilHelper.MD5Encrypt(password);
            business.ObjectId = Guid.NewGuid().ToString().ToLower();
            Context.SaveChanges();
            return password;
        }

        public List<Tuple<int, string>> GetStoresOnlyId(int chainId)
        {
            return Context.Businesses.Where(a => a.Category == BusinessCategory.Store && a.ParentId == chainId)
                .OrderBy(a => a.ID).Select(a => new Tuple<int, string>(a.ID, a.Name)).ToList();
        }

        public List<Order> GetOrders(int chainId, int businessId, OrderStatus? status, PagingQuery query, DateTime startDate, DateTime endDate)
        {
            var end = endDate.AddDays(1);
            IQueryable<Order> sql = Context.Orders;
            if (businessId == 0)
            {
                var ids = Context.Businesses.Where(a => a.ParentId == chainId).Select(a => a.ID).ToList();
                sql = sql.Where(a => ids.Contains(a.BusinessId.Value));
            }
            else
            {
                sql = sql.Where(a => a.BusinessId == businessId);
            }
            if (status != null && status > 0)
            {
                sql = sql.Where(a => (a.Status & status.Value) > 0);
            }
            sql = sql.Where(a => a.CreateTime >= startDate && a.CreateTime < end);
            query.RecordCount = sql.Count();
            sql = sql.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize);
            return sql.ToList();

            //var condition = string.Empty;
            //if (businessId == 0)
            //{
            //    condition = $@"[BusinessId] in (select id from dbo.[Business] where ParentId={chainId} and Category=0)";
            //}
            //else
            //{
            //    condition = $@"[BusinessId] = {businessId}";
            //}
            //if (status != null && status > 0)
            //{
            //    condition += $" and [Status] & {(int)status.Value} > 0 ";
            //}
            //condition += $" and [CreateTime] >= '{startDate.ToString("yyyy-MM-dd")}' and [CreateTime] < '{endDate.AddDays(1).ToString("yyyy-MM-dd")}'";

            //var countSql = $"select count(*) from dbo.[Order] where {condition}";
            //var textSql = $@"select [Identifier], [BusinessId], [OrderCode], [CreateTime], [Status], [Price], [ReceiverName], [Phone], [ReceiverAddress] from dbo.[Order] where {condition} 
            //                order by id offset {(query.PageIndex - 1) * query.PageSize} rows fetch next {query.PageSize} rows only";
            //query.RecordCount = ExecuteScalar<int>(countSql);
            //return ExecuteReader<Order>(textSql);
        }

        public List<Report_ChainSummary> GetBusinessSummary(int chainId, int businessId, DateTime startDate, DateTime endDate)
        {
            var end = endDate.AddDays(1);
            var queryOrder = Context.Orders.Where(a => (a.Status & OrderStatus.Valid) > 0 && a.CreateTime >= startDate && a.CreateTime < end);
            var queryBusiness = Context.Businesses.Where(a => a.ParentId == chainId);
            if (businessId > 0)
            {
                queryBusiness = queryBusiness.Where(a => a.ID == businessId);
            }
            var sql = from business in queryBusiness
                      join order in queryOrder
                      on business.ID equals order.BusinessId into joinOrder
                      from order in joinOrder.DefaultIfEmpty()
                      group order by new { business.ID, business.Name } into g
                      select new Report_ChainSummary
                      {
                          Id = g.Key.ID,
                          Name = g.Key.Name,
                          Amount = g.Sum(a => a == null ? 0 : a.Price == null ? 0 : a.Price.Value),
                          Quantity = g.Count(a => a != null)
                      };
            return sql.ToList();


            //var condition = string.Empty;
            //if (businessId > 0)
            //{
            //    condition += $@"a.Id={businessId} and ";
            //}
            //condition += $"a.ParentId={chainId} and b.Status & 2781 > 0 and b.CreateTime >= '{startDate:yyyy-MM-dd}' and b.CreateTime < '{endDate.AddDays(1):yyyy-MM-dd}'";
            //var sql = $@"select a.Id, a.Name, COUNT(*) as Quantity, SUM(b.Price) as Amount from dbo.[Business] a 
            //          left join dbo.[Order] b on a.Id=b.BusinessId
            //         where {condition}
            //         group by a.Id, a.Name";
            //return ExecuteReader<Report_ChainSummary>(sql);
        }

        public List<Report_UserList> GetUserListByChain(int id, PagingQuery query)
        {
            var data = from user in Context.Users
                       join business in Context.Businesses
                       on user.BusinessId equals business.ID
                       where user.IsRegister && business.ParentId == id
                       orderby user.ID descending
                       select new Report_UserList
                       {
                           Face = user.AvatarUrl,
                           Nickname = user.NickName,
                           Gender = user.Gender,
                           Area = user.Province + "" + user.City,
                           Phone = user.Phone,
                           Quantity = user.PurchaseTimes,
                           RegisteTime = user.CreateTime,
                           StoreName = business.Name
                       };
            query.RecordCount = data.Count();
            return data.Skip(query.PageSize * (query.PageIndex - 1))
                            .Take(query.PageSize).ToList();

        }
        public bool YcfkSave(Business business)
        {
            var entity = Context.Businesses.FirstOrDefault(a => a.ID == business.ID);
            if (entity == null) return false;
            entity.YcfkKey = business.YcfkKey;
            entity.YcfkSecret = business.YcfkSecret;
            Context.SaveChanges();
            return true;
        }

    }
}
