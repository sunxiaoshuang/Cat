using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JdCat.Cat.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 系统数据
        /// </summary>
        protected AppData AppData { get; }
        private Business _business;
        protected Business Business
        {
            get
            {
                if(_business == null)
                {
                    _business = HttpContext.Session.Get<Business>("User_Session");
                }
                return _business;
            }
        }
        public BaseController(AppData appData)
        {
            this.AppData = appData;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("User_Sesssion");
            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectResult("/Login");
            }
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewBag.CompanyName = AppData.Name;
            base.OnActionExecuted(context);
        }
    }
}
