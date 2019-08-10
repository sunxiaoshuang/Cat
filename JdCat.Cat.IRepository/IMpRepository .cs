using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    /// <summary>
    /// 公众号管理端接口
    /// </summary>
    public interface IMpRepository : IBaseRepository<Business>
    {
        /// <summary>
        /// 根据用户openid获取用户绑定的商户
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        Task<List<Business>> GetBusinessForOpenIdAsync(string openid);

    }
}
