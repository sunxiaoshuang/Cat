using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi.Controllers
{
    public class BaseController<T, TEntity> : Controller
        where TEntity : BaseEntity, new()
        where T : IBaseRepository<TEntity>
    {
        protected T Service { get; }

        public BaseController(T service)
        {
            this.Service = service;
        }
        private static readonly ILog log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(BaseController<T, TEntity>));
        public ILog Log
        {
            get
            {
                return log;
            }
        }

        public override JsonResult Json(object data)
        {
            return base.Json(data, AppData.JsonSetting);
        }


    }
}
