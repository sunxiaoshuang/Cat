using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
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
            entity.IsAutoReceipt = business.IsAutoReceipt;
            entity.Freight = business.Freight;
            entity.Description = business.Description;
            entity.Range = business.Range;
            entity.LogoSrc = business.LogoSrc;
            entity.BusinessLicense = business.BusinessLicense;
            return Context.SaveChanges() > 0;
        }

        public bool SaveSmall(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.AppId = business.AppId;
            entity.Secret = business.Secret;
            return Context.SaveChanges() > 0;
        }

        public bool SaveDada(Business business)
        {
            var entity = new Business { ID = business.ID };
            Context.Attach(entity);
            entity.DadaAppKey = business.DadaAppKey;
            entity.DadaAppSecret = business.DadaAppSecret;
            entity.DadaSourceId = business.DadaSourceId;
            entity.DadaShopNo = business.DadaShopNo;
            entity.CityCode = business.CityCode;
            entity.CityName = business.CityName;
            return Context.SaveChanges() > 0;
        }

        public async Task<string> UploadLogoAsync(string url, int businessId, string filename, string source)
        {

            using (var hc = new HttpClient())
            {
                var param = JsonConvert.SerializeObject(new
                {
                    BusinessId = businessId,
                    Name = filename,
                    Image400 = source
                });
                var httpcontent = new StringContent(param);
                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var msg = await hc.PostAsync(url, httpcontent);
                if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "ok";
                }
                return await msg.Content.ReadAsStringAsync();
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
    }
}
