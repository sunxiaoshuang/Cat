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
        public bool SaveBase(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.Name = business.Name;
            entity.Email = business.Email;
            entity.Address = business.Address;
            entity.Contact = business.Contact;
            entity.Mobile = business.Mobile;
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
        public bool BindPrintDevice(Business business, FeyinDevice device)
        {
            device.MemberCode = business.FeyinMemberCode;
            device.ApiKey = business.FeyinApiKey;
            device.BusinessId = business.ID;
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

        public bool SetDefaultPrinter(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.DefaultPrinterDevice = business.DefaultPrinterDevice;
            return Context.SaveChanges() > 0;
        }

        public List<Report_Order> GetOrderTotal(Business business, DateTime startTime, DateTime endTime)
        {
            return ExecuteReader<Report_Order>($@"select CreateTime, SUM(Price) Price, COUNT(*) Quantity from 
    (select Price, CONVERT(varchar(10), CreateTime, 120) as CreateTime from dbo.[Order] where BusinessId={business.ID} and CreateTime between '{startTime:yyyy-MM-dd}' and '{endTime:yyyy-MM-dd}')t1
group by CreateTime");
        }

        public List<Report_Product> GetProductTop10(Business business, DateTime date)
        {
            return ExecuteReader<Report_Product>($@"
            select top 10 b.ID, b.Name, SUM(a.Quantity) Quantity from dbo.[OrderProduct] a
	            inner join dbo.[Product] b on a.ProductId = b.ID
				inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            where b.BusinessId = {business.ID} and CONVERT(varchar(10), a.CreateTime, 120) = '{date:yyyy-MM-dd}'
            group by b.ID, b.Name
            order by Quantity desc
            ");
        }
        public List<Report_ProductPrice> GetProductPriceTop10(Business business, DateTime date)
        {
            return ExecuteReader<Report_ProductPrice>($@"
            select top 10 b.ID, b.Name, SUM(a.Quantity * a.Price) Amount from dbo.[OrderProduct] a
	            inner join dbo.[Product] b on a.ProductId = b.ID
				inner join dbo.[Order] c on a.OrderId = c.Id and c.Status & {(int)OrderStatus.Invalid} = 0
            where b.BusinessId = {business.ID} and CONVERT(varchar(10), a.CreateTime, 120) = '{date:yyyy-MM-dd}'
            group by b.ID, b.Name
            order by Amount desc
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
    }
}
