using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using System.Linq;

namespace JdCat.Cat.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CatDbContext context) : base(context)
        {
        }


        public IEnumerable<ProductType> GetTypes(Business business)
        {
            return _context.ProductTypes.Where(a => a.BusinessId == business.ID).ToList();
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

    }
}
