using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        /// <summary>
        /// 根据商户获取商品类别列表
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<ProductType> GetTypes(Business business);
        /// <summary>
        /// 新增商品类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<ProductType> AddTypes(IEnumerable<ProductType> type);
        /// <summary>
        /// 修改商品类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<ProductType> EditTypes(IEnumerable<ProductType> type);
        /// <summary>
        /// 删除商品类别
        /// </summary>
        /// <param name="type"></param>
        void RemoveTypes(IEnumerable<int> type);
        /// <summary>
        /// 指定的商品类别下是否存在商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ExistProduct(int id);
    }
}
