using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
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

        public List<Report_Product> GetProductTotal(Business business, DateTime date)
        {
            return ExecuteReader<Report_Product>($@"
            select top 10 b.ID, b.Name, SUM(a.Quantity) Quantity from dbo.[OrderProduct] a
	            inner join dbo.[Product] b on a.ProductId = b.ID
            where b.BusinessId = 1 and CONVERT(varchar(10), a.CreateTime, 120) = '{date:yyyy-MM-dd}'
            group by b.ID, b.Name
            order by Quantity desc
            ");
        }

    }
}
