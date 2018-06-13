using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Newtonsoft.Json;

namespace JdCat.Cat.Repository
{
    public class BusinessRepository : BaseRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(CatDbContext context) : base(context)
        {

        }

        public bool SaveBase(Business business)
        {
            var entity = new Business { ID = business.ID};
            Context.Attach(entity);
            entity.Name = business.Name;
            entity.Email = business.Email;
            entity.Address = business.Address;
            entity.Contact = business.Contact;
            entity.Mobile = business.Mobile;
            entity.IsAutoReceipt = business.IsAutoReceipt;
            entity.Freight = business.Freight;
            entity.Description = business.Description;
            entity.LogoSrc = business.LogoSrc;
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
    }
}
