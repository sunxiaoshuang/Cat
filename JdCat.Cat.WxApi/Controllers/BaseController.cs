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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi.Controllers
{
    public class BaseController<T, TEntity> : Controller
        where TEntity : BaseEntity, new()
        where T : IBusinessRepository<TEntity>
    {
        protected T Service { get; }

        public BaseController(T service)
        {
            this.Service = service;
        }

        public override JsonResult Json(object data)
        {
            return base.Json(data, AppData.JsonSetting);
        }


    }
}
