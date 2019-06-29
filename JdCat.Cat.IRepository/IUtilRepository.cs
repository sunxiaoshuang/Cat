using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;

namespace JdCat.Cat.IRepository
{
    public interface IUtilRepository : IBaseRepository<Business>
    {
        /// <summary>
        /// 处理微信回调事件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task<object> WxMsgHandlerAsync(WxEvent e); 
    }
}
