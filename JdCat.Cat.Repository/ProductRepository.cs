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
using System.Linq.Expressions;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CatDbContext context) : base(context)
        {
        }
        public IEnumerable<ProductType> GetTypes(Business business, ProductStatus? status = null)
        {
            //var types = Context.Products
            //    .Include(a => a.ProductType)
            //    .Include(a => a.Attributes)
            //    .Include(a => a.Formats)
            //    .Include(a => a.Images)
            //    .Where(a => a.Status == ProductStatus.Sale && a.BusinessId == business.ID)
            //    .GroupBy(a => a.ProductType).ToList();
            //var list = new List<ProductType>();
            //types.ForEach(a =>
            //{
            //    list.Add(a.Key);
            //});
            //return list;

            var list = Context.ProductTypes
                .Include(a => a.Products)
                .Include("Products.Attributes")
                .Include("Products.Formats")
                .Include("Products.Images")
                .Where(a => a.BusinessId == business.ID)
                .OrderBy(a => a.Sort).ToList();
            list.ForEach(a =>
            {
                var delList = new List<Product>();
                foreach (var item in a.Products)
                {
                    if (item.Status == ProductStatus.Delete)
                    {
                        delList.Add(item);
                        continue;
                    }
                    if (status != null)
                    {
                        if (item.Status != status)
                        {
                            delList.Add(item);
                        }
                    }
                }
                delList.ForEach(b => a.Products.Remove(b));
            });
            return list;
        }
        public IEnumerable<ProductType> GetTypes(int businessId)
        {
            var list = Context.ProductTypes
                .Include(a => a.Products)
                .Where(a => a.BusinessId == businessId)
                .OrderBy(a => a.Sort).ToList();
            list.ForEach(a =>
            {
                var delList = new List<Product>();
                foreach (var item in a.Products)
                {
                    if (item.Status == ProductStatus.Delete)
                    {
                        delList.Add(item);
                        continue;
                    }
                }
                delList.ForEach(b => a.Products.Remove(b));
            });
            return list;
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
            var list = Context.ProductTypes.Include(a => a.Products).Where(a => ids.Contains(a.ID));
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
        public List<Product> GetProducts(Business business, int? typeId, int pageIndex, out int count)
        {
            // 默认每页显示20条数据
            var pageSize = 20;
            var query = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .Where(a => a.BusinessId == business.ID && a.Status != ProductStatus.Delete);
            if (typeId.HasValue)
            {
                query = query.Where(a => a.ProductTypeId == typeId.Value);
            }
            count = query.Count();
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public Product GetProduct(int id)
        {
            return Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .First(a => a.ID == id);
        }
        public bool DeleteProduct(params int[] ids)
        {
            var products = Context.Products
                //.Include(a => a.Formats)
                //.Include(a => a.Images)
                //.Include(a => a.Attributes)
                .Where(a => ids.Contains(a.ID)).ToList();
            if (products.Count == 0) return false;
            foreach (var product in products)
            {
                //if (product.Images.Count > 0)
                //{
                //    DeleteImage(product.Images.First(), apiUrl, product.BusinessId);
                //}
                product.Status = ProductStatus.Delete;
            }
            return Context.SaveChanges() > 0;
        }
        public void DeleteImage(ProductImage image, string apiUrl, int businessId)
        {
            // 由于订单中可能使用到图片，所以不允许删除
            //Task.Run(async () =>
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var result = await client.DeleteAsync($"{apiUrl}/Product?name={image.Name}.{image.ExtensionName}&businessId={businessId}");
            //        var msg = await result.Content.ReadAsStringAsync();
            //    }
            //});
        }
        public void DeleteImage(ProductImage image)
        {
            Context.ProductImages.Remove(image);
        }

        public Product Update(Product product)
        {
            var entity = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Attributes)
                .First(a => a.ID == product.ID);
            entity.Description = product.Description;
            entity.MinBuyQuantity = product.MinBuyQuantity;
            entity.Name = product.Name;
            entity.ProductTypeId = product.ProductTypeId;
            entity.UnitName = product.UnitName;
            if (product.Images != null && product.Images.Count > 0)
            {
                entity.Images = new List<ProductImage>();
                foreach (var img in product.Images.Where(a => a.ID == 0))
                {
                    entity.Images.Add(img);
                }
            }
            var removeFormats = entity.Formats.Where(a => product.Formats.FirstOrDefault(b => b.ID == a.ID) == null).ToList();
            foreach (var removeFormat in removeFormats)
            {
                entity.Formats.Remove(removeFormat);
            }
            foreach (var format in product.Formats)
            {
                if (format.ID == 0)
                {
                    entity.Formats.Add(format);
                    continue;
                }
                var old = entity.Formats.First(a => a.ID == format.ID);
                old.Name = format.Name;
                old.PackingPrice = format.PackingPrice;
                old.PackingQuantity = format.PackingQuantity;
                old.Position = format.Position;
                old.Price = format.Price;
                old.SKU = format.SKU;
                old.Stock = old.Stock;
                old.UPC = old.UPC;
            }
            var removeAttrs = entity.Attributes.Where(a => product.Attributes.FirstOrDefault(b => b.ID == a.ID) == null).ToList();
            foreach (var removeAttr in removeAttrs)
            {
                entity.Attributes.Remove(removeAttr);
            }
            foreach (var attr in product.Attributes)
            {
                if (attr.ID == 0)
                {
                    entity.Attributes.Add(attr);
                    continue;
                }
                var old = entity.Attributes.First(a => a.ID == attr.ID);
                old.Name = attr.Name;
                old.Item1 = attr.Item1;
                old.Item2 = attr.Item2;
                old.Item3 = attr.Item3;
                old.Item4 = attr.Item4;
                old.Item5 = attr.Item5;
                old.Item6 = attr.Item6;
                old.Item7 = attr.Item7;
                old.Item8 = attr.Item8;
            }
            Context.SaveChanges();
            return entity;
        }
        public void Up(int id)
        {
            var product = new Product { ID = id };
            product.Status = ProductStatus.Init;
            Context.Attach(product);
            product.ModifyTime = DateTime.Now;
            product.PublishTime = DateTime.Now;
            product.Status = ProductStatus.Sale;
        }
        public void Down(int id)
        {
            var product = new Product { ID = id };
            Context.Attach(product);
            product.ModifyTime = DateTime.Now;
            product.NotSaleTime = DateTime.Now;
            product.Status = Model.Enum.ProductStatus.NotSale;
        }
    }
}
