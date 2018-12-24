using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.IRepository
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        /// <summary>
        /// 获取商品类型列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        List<ProductType> GetProductTypes(int businessId);
        /// <summary>
        /// 根据商户获取商品类别列表
        /// </summary>
        /// <param name="business"></param>
        /// <param name="status">商品状态</param>
        /// <returns></returns>
        IEnumerable<ProductType> GetTypes(Business business, ProductStatus? status = null);
        /// <summary>
        /// 根据商户Id获取商品类别列表
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        IEnumerable<ProductType> GetTypes(int businessId);
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
        /// <param name="business">商户对象</param>
        /// <param name="typeId">商品类别id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="count">记录数</param>
        /// <returns></returns>
        List<Product> GetProducts(Business business, int? typeId, int pageIndex, out int count);
        /// <summary>
        /// 获取所有产品
        /// </summary>
        /// <returns></returns>
        List<Product> GetProducts(Business business);
        /// <summary>
        /// 根据商品id获取商品折扣
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SaleProductDiscount GetDiscount(int id);
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Product GetProduct(int id);
        /// <summary>
        /// 获取套餐商品
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<KeyValuePair<int, string>> GetSetMealProducts(params int[] ids);
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="ids">商品id</param>
        /// <returns></returns>
        bool DeleteProduct(params int[] ids);
        /// <summary>
        /// 删除产品图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="apiUrl"></param>
        /// <param name="businessId"></param>
        void DeleteImage(ProductImage image, string apiUrl, int businessId);
        /// <summary>
        /// 删除产品图片记录
        /// </summary>
        /// <param name="image"></param>
        void DeleteImage(ProductImage image);
        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Product Update(Product product);
        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void Up(int id);
        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void Down(int id);
        /// <summary>
        /// 查询出商户非套餐商品
        /// </summary>
        /// <returns></returns>
        List<ProductType> GetProductWithoutSetMeal(int id);
        /// <summary>
        /// 获取连锁门店树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<object> GetStoreTree(int id);
        /// <summary>
        /// 获取商户商品树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<object> GetProductTree(int id);
        /// <summary>
        /// 复制商品
        /// </summary>
        /// <param name="storeIds">商户id</param>
        /// <param name="productIds">商品id</param>
        /// <param name="imageUrl">上传服务接口地址</param>
        /// <returns>一共复制多少个商品</returns>
        Task<int> Copy(CopyProduct copyData, string imageUrl);
    }
}
