﻿using System;
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

namespace JdCat.Cat.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CatDbContext context) : base(context)
        {
        }
        public IEnumerable<ProductType> GetTypes(Business business)
        {
            return _context.ProductTypes.Include(a => a.Products).Where(a => a.BusinessId == business.ID).OrderBy(a => a.Sort).ToList();
        }
        public IEnumerable<ProductType> AddTypes(IEnumerable<ProductType> types)
        {
            foreach (var item in types)
            {
                item.CreateTime = DateTime.Now;
                _context.Add(item);
            }
            return types;
        }
        public IEnumerable<ProductType> EditTypes(IEnumerable<ProductType> types)
        {
            var ids = types.Select(a => a.ID);
            var set = _context.ProductTypes.Where(a => ids.Contains(a.ID)).ToList();
            foreach (var item in set)
            {
                var newEntity = types.First(a => a.ID == item.ID);
                item.Name = newEntity.Name;
                item.Sort = newEntity.Sort;
                item.Description = newEntity.Description;
                _context.Update(item);
            }
            return set;
        }
        public void RemoveTypes(IEnumerable<int> ids)
        {
            var list = _context.ProductTypes.Where(a => ids.Contains(a.ID));
            _context.ProductTypes.RemoveRange(list);
        }
        public bool ExistProduct(int id)
        {
            return _context.Products.Count(a => a.ProductTypeId == id) > 0;
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
            return _context.SettingProductAttributes.Include(a => a.Childs).Where(a => a.Level == 1);
        }

    }
}
