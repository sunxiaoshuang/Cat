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
        public async Task<bool> BindPrintDevice(Business business, FeyinDevice device)
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
                if(ret.ret != 0)
                {
                    return false;
                }
            }

            Context.FeyinDevices.Add(device);
            return Context.SaveChanges() > 0;
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
            return ExecuteReader<Report_Order>($@"select CreateTime, SUM(Price) Price, COUNT(*) Quantity from 
    (select Price, CONVERT(varchar(10), CreateTime, 120) as CreateTime from dbo.[Order] where BusinessId={business.ID} and Status & {(int)OrderStatus.Valid} > 0 and CreateTime between '{startTime:yyyy-MM-dd}' and '{endTime:yyyy-MM-dd}')t1
group by CreateTime");
        }

        public List<Report_Product> GetProductTop10(Business business, DateTime date)
        {
            return ExecuteReader<Report_Product>($@"
            select top 10 b.ID, b.Name, sum(a.Quantity) Quantity from dbo.[OrderProduct] a
	            inner join dbo.[Product] b on a.ProductId = b.ID
				inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            where b.BusinessId = {business.ID} and convert(varchar(10), a.CreateTime, 120) = '{date:yyyy-MM-dd}'
            group by b.ID, b.Name
            order by Quantity desc
            ");
        }
        public List<Report_ProductPrice> GetProductPriceTop10(Business business, DateTime date)
        {
            return ExecuteReader<Report_ProductPrice>($@"
            select top 10 b.ID, b.Name, sum(a.Price) Amount from dbo.[OrderProduct] a
	            inner join dbo.[Product] b on a.ProductId = b.ID
				inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            where b.BusinessId = {business.ID} and convert(varchar(10), a.CreateTime, 120) = '{date:yyyy-MM-dd}'
            group by b.ID, b.Name
            order by Amount desc
            ");
        }

        public List<Report_SaleStatistics> GetSaleStatistics(Business business, DateTime start, DateTime end)
        {
            return ExecuteReader<Report_SaleStatistics>($@"
                select CreateTime AS [Date], sum(Price) Total, count(CreateTime) Quantity from (
	                select Price, convert(VARCHAR(10), CreateTime, 120) CreateTime from dbo.[Order] where 
	                [BusinessId] = {business.ID} and 
	                [Status] & {(int)OrderStatus.Valid} > 0 and 
	                [CreateTime] between '{start:yyyy-MM-dd}' and '{end.AddDays(1):yyyy-MM-dd}' 
                )t
                group by t.CreateTime
                order by CreateTime desc
            ");
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
    }
}
