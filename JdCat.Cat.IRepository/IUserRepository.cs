using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// 根据openId获取用户
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        User Get(string openId);
        /// <summary>
        /// 授权用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User GrantInfo(User user);
        /// <summary>
        /// 授权手机号
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool GrantPhone(int id, string phone);
    }
}
