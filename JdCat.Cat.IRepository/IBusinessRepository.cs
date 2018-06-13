using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IBusinessRepository : IBaseRepository<Business>
    {
        /// <summary>
        /// 保存商户基本信息
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool SaveBase(Business business);
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
