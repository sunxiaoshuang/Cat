using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using JdCat.Cat.Common;

namespace JdCat.Cat.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CatDbContext context) : base(context)
        {
        }
        public IEnumerable<ProductType> GetTypes(Business business)
        {
            return Context.ProductTypes.Include(a => a.Products).Where(a => a.BusinessId == business.ID).OrderBy(a => a.Sort).ToList();
        }
        public IEnumerable<ProductType> AddTypes(IEnumerable<ProductType> types)
        {
            foreach (var item in types)
            {
                item.CreateTime = DateTime.Now;
                Context.Add(item);
            }
            return types;
        }
        public IEnumerable<ProductType> EditTypes(IEnumerable<ProductType> types)
        {
            var ids = types.Select(a => a.ID);
            var set = Context.ProductTypes.Where(a => ids.Contains(a.ID)).ToList();
            foreach (var item in set)
            {
                var newEntity = types.First(a => a.ID == item.ID);
                item.Name = newEntity.Name;
                item.Sort = newEntity.Sort;
                item.Description = newEntity.Description;
                Context.Update(item);
            }
            return set;
        }
        public void RemoveTypes(IEnumerable<int> ids)
        {
            var list = Context.ProductTypes.Where(a => ids.Contains(a.ID));
            Context.ProductTypes.RemoveRange(list);
        }
        public bool ExistProduct(int id)
        {
            return Context.Products.Count(a => a.ProductTypeId == id) > 0;
        }

        public async Task<string> UploadImageAsync(string url, int businessId, string name, string img400, string img200, string img100)
        {
            using (var hc = new HttpClient())
            {
                var param = JsonConvert.SerializeObject(new
                {
                    BusinessId = businessId,
                    Name = name,
                    Image400 = img400,
                    Image200 = img200,
                    Image100 = img100,
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

        public IEnumerable<SettingProductAttribute> GetAttributes()
        {
            return Context.SettingProductAttributes.Include(a => a.Childs).Where(a => a.Level == 1);
        }

        public List<Product> GetProducts(Business business, int? typeId, int pageIndex)
        {
            // 默认每页显示20条数据
            var pageSize = 20;

            var query = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .Where(a => a.BusinessId == business.ID);
            if(typeId.HasValue)
            {
                query = query.Where(a => a.ProductTypeId == typeId.Value);
            }
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool DeleteProduct(int id, string apiUrl)
        {
            var product = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .FirstOrDefault(a => a.ID == id);
            if (product == null) return true;
            if(product.Images.Count > 0)
            {
                Task.Run(async () =>
                {
                    using (var client = new HttpClient())
                    {
                        var img = product.Images.FirstOrDefault();
                        var result = await client.DeleteAsync($"{apiUrl}/Product?name={img.Name}.{img.ExtensionName}&businessId={product.BusinessId}");
                        var msg = await result.Content.ReadAsStringAsync();

                    }
                });
            }
            Context.Products.Remove(product);
            return Context.SaveChanges() > 0;
        }

    }
}
