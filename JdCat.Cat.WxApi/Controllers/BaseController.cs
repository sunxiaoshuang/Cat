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
        where T : IBaseRepository<TEntity>
    {
        protected const string SessionName = "Session_User";
        private User _loginUser;

        protected User LoginUser
        {
            get => _loginUser ?? (_loginUser = HttpContext.Session.Get<User>(SessionName));
            set => HttpContext.Session.Set(SessionName, value);
        }
        protected T Service { get; }

        public BaseController(T service)
        {
            this.Service = service;
        }


    }
}
