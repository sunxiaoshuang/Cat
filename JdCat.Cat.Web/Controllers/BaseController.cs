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

namespace JdCat.Cat.Web.Controllers
{
    public class BaseController<T, TEntity> : Controller 
        where TEntity : BaseEntity, new()
        where T: IBaseRepository<TEntity>
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
                if (_business == null)
                {
                    _business = HttpContext.Session.Get<Business>(AppData.Session);
                }
                return _business;
            }
        }
        private T _service;
        protected T Service => _service;
        public BaseController(AppData appData, T service)
        {
            this.AppData = appData;
            this._service = service;

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 取用户会话信息
            var user = HttpContext.Session.GetString(AppData.Session);
            if (string.IsNullOrEmpty(user))
            {
                // 不存在则去用户cookie，如果也不存在则跳转到登录页
                var id = HttpContext.Request.Cookies[AppData.Cookie];
                if (string.IsNullOrEmpty(id))
                {
                    context.Result = new RedirectResult("~/Login");
                }
                else
                {
                    // 根据id获取用户信息，自动登录
                    //var response = GetAsync($"/business/{id}");
                    //response.Wait();
                    //var content = response.Result;

                    //using (var db = new CatDbContext(new DbContextOptionsBuilder<CatDbContext>().UseSqlServer(AppData.Connection).Options))
                    //{
                    //    _business = db.Find<Business>(int.Parse(id));
                    //    if (_business == null)
                    //    {
                    //        context.Result = new RedirectResult("~/Login");
                    //    }
                    //    HttpContext.Session.Set(AppData.Session, _business);
                    //}
                    _business = Service.Set<Business>().FirstOrDefault(a => a.ID == int.Parse(id));
                    if (_business == null)
                    {
                        context.Result = new RedirectResult("~/Login");
                    }
                    HttpContext.Session.Set(AppData.Session, _business);
                }
            }
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewBag.CompanyName = AppData.Name;
            base.OnActionExecuted(context);
        }

        /* 不要了
        #region 请求方法

        protected async Task<string> GetAsync(string uri)
        {
            using (var hc = new HttpClient())
            {
                var response = await hc.GetAsync(AppData.ApiUri + uri);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        protected async Task<string> PostAsync(string uri, HttpContent formdata)
        {
            using (var hc = new HttpClient())
            {
                var response = await hc.PostAsync(AppData.ApiUri + uri, formdata);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        protected async Task<string> PutAsync(string uri, HttpContent formdata)
        {
            using (var hc = new HttpClient())
            {
                var response = await hc.PutAsync(AppData.ApiUri + uri, formdata);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        protected async Task<string> DeleteAsync(string uri)
        {
            using (var hc = new HttpClient())
            {
                var response = await hc.DeleteAsync(AppData.ApiUri + uri);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        #endregion
        */
    }
}
