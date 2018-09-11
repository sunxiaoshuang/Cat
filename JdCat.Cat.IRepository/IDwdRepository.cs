﻿using System;
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
    public interface IDwdRepository : IBaseRepository<DWD_Business>
    {
        /// <summary>
        /// 创建点我达商户
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        bool CreateShop(DWD_Business business);
    }
}
