using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface ISessionDataRepository : IBaseRepository<SessionData>
    {
        /// <summary>
        /// 设置登录Session
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SessionData SetSession(SessionData entity);
        /// <summary>
        /// 根据id获取Session对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SessionData Get(int id);

    }
}
