using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        /// <summary>
        /// 上传商品图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="businessId"></param>
        /// <param name="name"></param>
        /// <param name="img400"></param>
        /// <param name="img200"></param>
        /// <param name="img100"></param>
        /// <returns></returns>
        Task<string> UploadImageAsync(string url, int businessId, string name, string img400, string img200, string img100);
        /// <summary>
        /// 获取商品属性列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<SettingProductAttribute> GetAttributes();
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="business"></param>
        /// <param name="typeId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        List<Product> GetProducts(Business business, int? typeId, int pageIndex);
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Product GetProduct(int id);
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="apiUrl">图片服务器的地址</param>
        /// <returns></returns>
        bool DeleteProduct(int id, string apiUrl);
        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Up(int id);
        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Down(int id);
    }
}
