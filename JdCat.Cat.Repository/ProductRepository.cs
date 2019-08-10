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
using System.Dynamic;
using JdCat.Cat.Common.Models;

namespace JdCat.Cat.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CatDbContext context) : base(context)
        {
        }
        public List<ProductType> GetProductTypes(int businessId)
        {
            return Context.ProductTypes.Where(a => a.BusinessId == businessId).ToList();
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

            //var list = Context.ProductTypes
            //    .Include(a => a.Products)
            //    .Include("Products.Attributes")
            //    .Include("Products.Formats")
            //    .Include("Products.Images")
            //    .Where(a => a.BusinessId == business.ID)
            //    .OrderBy(a => a.Sort).ToList();

            //list.ForEach(a =>
            //{
            //    var delList = new List<Product>();
            //    foreach (var item in a.Products)
            //    {
            //        if (item.Status == ProductStatus.Delete)
            //        {
            //            delList.Add(item);
            //            continue;
            //        }
            //        if (status != null)
            //        {
            //            if (item.Status != status)
            //            {
            //                delList.Add(item);
            //            }
            //        }
            //    }
            //    delList.ForEach(b => a.Products.Remove(b));
            //});

            //var query = from type in Context.ProductTypes
            //join product in Context.Products on type.ID equals product.ProductTypeId
            //join attr in Context.ProductAttributes on product.ID equals attr.ProductId
            //join format in Context.ProductFormats on product.ID equals format.ProductId
            //join image in Context.ProductImages on product.ID equals image.ProductId
            //where type.BusinessId == business.ID && product.Status != ProductStatus.Delete && !format.IsDelete
            //orderby type.Sort
            //select type;
            //var list = query.ToList();

            var query = Context.ProductTypes
                .AsNoTracking()
                .Include(a => a.Products)
                .Include("Products.Attributes")
                .Include("Products.Formats")
                .Include("Products.Images")
                .Where(a => a.BusinessId == business.ID)
                .Select(a => new
                {
                    Type = a,
                    Products = a.Products
                    .Where(b => b.Status != ProductStatus.Delete)
                    .Select(c => new
                    {
                        Product = c,
                        Formats = c.Formats.Where(format => !format.IsDelete),
                        c.Images,
                        c.Attributes
                    })
                }).OrderBy(a => a.Type.Sort);

            var list = query
                .ToList()
                .Select(a =>
                {
                    var type = a.Type;
                    if (type.Products != null)
                    {
                        var products = a.Products.Select(b =>
                        {
                            var product = b.Product;
                            product.Attributes = b.Attributes;
                            product.Images = b.Images;
                            product.Formats = b.Formats.ToList();
                            return product;
                        });
                        if (status != null)
                        {
                            products = products.Where(b => b.Status == status);
                        }

                        type.Products = products.ToList();
                    }
                    return type;
                });

            return list;
        }


        public async Task<object> GetTakeoutMenusAsync(int id)
        {
            var list = await Context.ProductTypes
                .AsNoTracking()
                .Include(a => a.Products)
                .Include("Products.Attributes")
                .Include("Products.Formats")
                .Include("Products.Images")
                .Where(a => a.BusinessId == id)
                .Select(a => new
                {
                    a.ID,
                    a.Name,
                    a.Sort,
                    Products = a.Products.Where(b => b.Status == ProductStatus.Sale && (b.Scope & ActionScope.Takeout) > 0)
                    .Select(b => new
                    {
                        b.ID,
                        b.MinBuyQuantity,
                        b.Name,
                        b.ProductIdSet,
                        b.ProductTypeId,
                        b.UnitName,
                        b.Feature,
                        Description = b.Description ?? "",
                        Formats = b.Formats.Where(c => !c.IsDelete).Select(c => new { c.ID, c.Name, c.Code, c.Price, c.PackingPrice, c.PackingQuantity, c.ProductId, c.SKU, c.Stock, c.UPC }),
                        Attributes = b.Attributes.Select(c => new { c.ID, c.Item1, c.Item2, c.Item3, c.Item4, c.Item5, c.Item6, c.Item7, c.Item8, c.Name }),
                        Images = b.Images.Select(c => new { c.ID, c.Name, c.ExtensionName })
                    })
                })
                .OrderBy(a => a.Sort)
                .ToListAsync();

            return list.Where(a => a.Products.Count() > 0).ToList();
        }

        public IEnumerable<ProductType> GetTypes(int businessId)
        {
            //var list = Context.ProductTypes
            //    .Include(a => a.Products)
            //    .Where(a => a.BusinessId == businessId)
            //    .OrderBy(a => a.Sort).ToList();
            //list.ForEach(a =>
            //{
            //    var delList = new List<Product>();
            //    foreach (var item in a.Products)
            //    {
            //        if (item.Status == ProductStatus.Delete)
            //        {
            //            delList.Add(item);
            //            continue;
            //        }
            //    }
            //    delList.ForEach(b => a.Products.Remove(b));
            //});

            var list = Context.ProductTypes
                .AsNoTracking()
                .Include(a => a.Products)
                .Where(a => a.BusinessId == businessId)
                .Select(a => new { Type = a, Products = a.Products.Where(b => b.Status != ProductStatus.Delete) })
                .OrderBy(a => a.Type.Sort)
                .ToList()
                .Select(a =>
                {
                    var type = a.Type;
                    type.Products = a.Products.ToList();
                    return type;
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
        public string GetNextProductFormat()
        {
            return ExecuteScalar("SELECT CONCAT('F', DATE_FORMAT(NOW(),'%Y'), fn_right_padding(NEXT_VAL('FormatNumbers'), 9))") + "";
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
        public List<Product> GetProducts(Business business)
        {
            //var query = Context.Products
            //    .Include(a => a.Formats)
            //    .Include(a => a.Images)
            //    .Include(a => a.Attributes)
            //    .Where(a => a.BusinessId == business.ID && a.Status != ProductStatus.Delete);
            //return query.ToList();
            var query = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .Where(a => a.BusinessId == business.ID && a.Status != ProductStatus.Delete)
                .Select(a => new { Product = a, Formats = a.Formats.Where(b => !b.IsDelete), a.Images, a.Attributes });

            return query.ToList()
                .Select(a =>
                {
                    a.Product.Formats = a.Formats?.ToList();
                    a.Product.Images = a.Images;
                    a.Product.Attributes = a.Attributes;
                    return a.Product;
                })
                .ToList();
        }
        public SaleProductDiscount GetDiscount(int id)
        {
            return Context.SaleProductDiscount.FirstOrDefault(a => a.ProductId == id && a.Status != ActivityStatus.Delete);
        }
        public Product GetProduct(int id)
        {
            var query = Context.Products
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Include(a => a.Attributes)
                .Where(a => a.ID == id)
            .Select(a => new { Product = a, Formats = a.Formats.Where(b => !b.IsDelete), a.Images, a.Attributes });
            return query.ToList()
                .Select(a =>
                {
                    a.Product.Formats = a.Formats?.ToList();
                    a.Product.Images = a.Images;
                    a.Product.Attributes = a.Attributes;
                    return a.Product;
                })
                .First();
        }
        public List<KeyValuePair<int, string>> GetSetMealProducts(params int[] ids)
        {
            var list = Context.Products
                 .Where(a => ids.Contains(a.ID))
                 .Select(a => new { a.ID, a.Name })
                 .ToList();
            return list.Select(a => new KeyValuePair<int, string>(a.ID, a.Name)).ToList();
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
            entity.Scope = product.Scope;
            entity.IsDiscount = product.IsDiscount;
            if (entity.ProductIdSet != product.ProductIdSet)
            {
                entity.ProductIdSet = product.ProductIdSet;
                var relatives = Context.ProductRelatives.Where(a => a.SetMealId == entity.ID).ToList();
                Context.RemoveRange(relatives);
                if (!string.IsNullOrEmpty(product.ProductIdSet))
                {
                    product.ProductIdSet.Split(',').ToList().ForEach(a =>
                    {
                        var item = new ProductRelative { SetMealId = product.ID, ProductId = Convert.ToInt32(a) };
                        Context.Add(item);
                    });
                }
            }
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
                //entity.Formats.Remove(removeFormat);
                removeFormat.IsDelete = true;
            }
            foreach (var format in product.Formats)
            {
                if (format.ID == 0)
                {
                    format.Code = GetNextProductFormat();
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
            product.Status = ProductStatus.NotSale;
        }
        public List<ProductType> GetProductWithoutSetMeal(int id)
        {
            var types = GetTypes(id).ToList();
            types.ForEach(a =>
            {
                var delList = new List<Product>();
                foreach (var item in a.Products)
                {
                    if (item.Feature == ProductFeature.SetMeal)
                    {
                        delList.Add(item);
                        continue;
                    }
                }
                delList.ForEach(b => a.Products.Remove(b));
            });
            return types;
        }
        public List<object> GetStoreTree(int id)
        {
            var result = new List<object>();
            var list = Context.Businesses.Where(a => a.ParentId == id)
                .Select(a => new { a.ID, a.Name, a.Province, a.City, a.Area }).ToList();
            var provinceGroup = list.GroupBy(a => a.Province).ToList();
            foreach (var item in provinceGroup)
            {
                dynamic province = new ExpandoObject();
                result.Add(province);
                province.name = item.Key;
                province.level = 1;
                province.list = new List<object>();
                var cityGroup = item.ToList().GroupBy(a => a.City).ToList();
                foreach (var item2 in cityGroup)
                {
                    dynamic city = new ExpandoObject();
                    province.list.Add(city);
                    city.name = item2.Key;
                    city.level = 2;
                    city.list = new List<object>();
                    var areaGroup = item2.ToList().GroupBy(a => a.Area).ToList();
                    foreach (var item3 in areaGroup)
                    {
                        dynamic area = new ExpandoObject();
                        city.list.Add(area);
                        area.name = item3.Key;
                        area.level = 3;
                        area.list = item3.ToList().Select(a => new { a.ID, a.Name });
                    }
                }
            }
            return result;
        }
        public async Task<object> GetProductTreeAsync(int id, bool isSetMeal = false)
        {
            var types = await Context.ProductTypes
                .AsNoTracking()
                .Include(a => a.Products)
                .Where(a => a.BusinessId == id)
                .Select(a => new
                {
                    a.Name,
                    a.ID,
                    List = a.Products.Where(b => b.Status != ProductStatus.Delete && (isSetMeal || b.Feature != ProductFeature.SetMeal)).Select(b => new { b.Name, b.ID, b.Status, b.Pinyin, b.FirstLetter, b.Code })
                }).ToListAsync();
            return types;
        }
        public async Task<int> Copy(CopyProduct copyData, string imageUrl)
        {
            // 注：复制的商品不包含：折扣、套餐

            // 1. 查找需要复制的商品
            var now = DateTime.Now;
            var products = Context.Products.AsNoTracking()
                .Include(a => a.ProductType)
                .Include(a => a.Attributes)
                .Include(a => a.Formats)
                .Include(a => a.Images)
                .Where(a => copyData.ProductIds.Contains(a.ID))
                .OrderBy(a => a.ProductType.Sort)
                .ThenBy(a => a.ID)
                .ToList();

            var imageNames = new List<string>();        // 需要复制的图片名称
            // 2. 将需要复制的商品属性初始化，Tag1用来保存商品类别名称
            products.ForEach(product =>
            {
                product.ID = 0;
                product.BusinessId = 0;
                product.CreateTime = now;
                product.ModifyTime = null;
                product.NotSaleTime = null;
                product.ProductIdSet = null;
                product.ProductTypeId = null;
                product.PublishTime = now;
                product.Code = null;
                if (product.ProductType != null)
                {
                    product.Tag1 = product.ProductType.Name;
                    product.ProductType = null;
                }
                if (product.Attributes != null)
                {
                    foreach (var attr in product.Attributes)
                    {
                        attr.CreateTime = now;
                        attr.ID = 0;
                        attr.ProductId = 0;
                    }
                }
                if (product.Formats != null)
                {
                    foreach (var format in product.Formats)
                    {
                        format.CreateTime = now;
                        format.ID = 0;
                        format.ProductId = 0;
                    }
                }
                if (product.Images != null)
                {
                    foreach (var img in product.Images)
                    {
                        img.CreateTime = now;
                        img.ID = 0;
                        img.ProductId = 0;
                        imageNames.Add(img.Name + "." + img.ExtensionName);
                    }
                }
            });
            // 3. 读取每个门店的商品类型
            var types = Context.ProductTypes
                .Where(a => copyData.StoreIds.Contains(a.BusinessId))
                .OrderBy(a => a.BusinessId)
                .ThenBy(a => a.Sort)
                .ToList();
            // 4. 复制商品
            foreach (var id in copyData.StoreIds)
            {
                var curTypes = types.Where(a => a.BusinessId == id).ToList();       // 当前的门店所有商品类别
                var maxSort = 0;
                if (curTypes.Count > 0)
                {
                    maxSort = curTypes.Max(a => a.Sort);
                }
                products.ForEach(a =>
                {
                    var product = (Product)a.Clone();
                    product.BusinessId = id;
                    var typeName = product.Tag1 + "";
                    if (typeName.Length > 0)
                    {
                        var type = curTypes.FirstOrDefault(b => b.Name == product.Tag1 + "");
                        if (type == null)
                        {
                            type = new ProductType { BusinessId = id, Description = "总店复制", Name = typeName, Sort = ++maxSort };
                            product.ProductType = type;
                            curTypes.Add(type);
                        }
                        else
                        {
                            product.ProductTypeId = type.ID;
                        }
                    }
                    Context.Add(product);
                    Context.SaveChanges();
                    product.Code = product.ID.ToString().PadLeft(6, '0');
                    Context.SaveChanges();
                });
            }
            var count = Context.SaveChanges();

            var postData = new CopyProduct
            {
                ChainId = copyData.ChainId,
                StoreIds = copyData.StoreIds,
                ImageNames = imageNames.ToArray()
            };
            var content = JsonConvert.SerializeObject(postData);
            try
            {
                using (var client = new HttpClient())
                using (var body = new StringContent(content))
                {
                    body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var res = await client.PostAsync(imageUrl, body);
                    res.EnsureSuccessStatusCode();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return count;
        }
        public async Task<object> GetProductsOnlyNameAsync(int businessId)
        {
            return await Context.Products.Where(a => a.Status != ProductStatus.Delete && a.BusinessId == businessId)
                .Select(a => new { a.ID, a.Name, a.Code, a.Pinyin, a.FirstLetter })
                .ToListAsync();
        }
    }
}
