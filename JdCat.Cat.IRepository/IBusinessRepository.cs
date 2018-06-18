using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IBusinessRepository : IBusinessRepository<Business>
    {
        /// <summary>
        /// 获取商户对象，不要跟踪状态
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Business GetBusiness(Expression<Func<Business, bool>> expression);
        /// <summary>
        /// 保存商户基本信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveBase(Business business);
        /// <summary>
        /// 保存小程序信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveSmall(Business business);
        /// <summary>
        /// 保存达达配置信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveDada(Business business);
        /// <summary>
        /// 上传商户LOGO
        /// </summary>
        /// <param name="url"></param>
        /// <param name="businessId"></param>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<string> UploadLogoAsync(string url, int businessId, string filename, string source);
    }
}
