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
    public interface IDwdRepository : IBaseRepository<DWDStore>
    {
        /// <summary>
        /// 创建点我达商户
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool CreateShop(DWDStore business);
        /// <summary>
        /// 创建点我达充值对象
        /// </summary>
        /// <param name="recharge"></param>
        /// <returns></returns>
        bool AddRecharge(DWD_Recharge recharge);
        /// <summary>
        /// 充值成功
        /// </summary>
        /// <param name="bizNo"></param>
        void RechargeSuccess(string bizNo);
    }
}
