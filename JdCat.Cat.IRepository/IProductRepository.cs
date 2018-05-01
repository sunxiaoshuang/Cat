using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        IEnumerable<ProductType> GetTypes(Business business);
        IEnumerable<ProductType> AddTypes(IEnumerable<ProductType> type);
        IEnumerable<ProductType> EditTypes(IEnumerable<ProductType> type);
        void RemoveTypes(IEnumerable<int> type);
    }
}
