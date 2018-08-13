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
using JdCat.Cat.Web.App_Code;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using log4net;

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
        protected Business Business => _business ?? (_business = HttpContext.Session.Get<Business>(AppData.Session));
        protected T Service { get; }

        public BaseController(AppData appData, T service)
        {
            this.AppData = appData;
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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var type = context.HttpContext.Request.Query["appType"].ToString();
            type = type == "" ? "app" : type;
            // 取用户会话信息
            var user = HttpContext.Session.GetString(AppData.Session);
            if (string.IsNullOrEmpty(user))
            {
                // 不存在则取用户cookie，如果也不存在则跳转到登录页
                var id = HttpContext.Request.Cookies[AppData.Cookie];
                if (string.IsNullOrEmpty(id))
                {
                    context.Result = new RedirectResult("~/Login?type=" + type);
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
                    _business = Service.Set<Business>().AsNoTracking().SingleOrDefault(a => a.ID == int.Parse(id));
                    if (_business == null)
                    {
                        context.Result = new RedirectResult("~/Login");
                    }
                    HttpContext.Session.Set(AppData.Session, _business);
                }
            }
            base.OnActionExecuting(context);
        }

        public override ViewResult View()
        {
            ViewLoad();
            return base.View();
        }
        public override ViewResult View(object model)
        {
            ViewLoad();
            return base.View();
        }

        public override JsonResult Json(object data)
        {
            return base.Json(data, AppData.JsonSetting);
        }

        private void ViewLoad()
        {
            ViewBag.apiUrl = AppData.FileUri;
            ViewBag.CompanyName = AppData.Name;
        }

        protected FeYinHelper GetPrintHelper()
        {
            var helper = HttpContext.RequestServices.GetService<FeYinHelper>();
            if (Business.FeyinMemberCode == helper.MemberCode)
            {
                return helper;
            }
            else
            {
                return new FeYinHelper
                {
                    MemberCode = Business.FeyinMemberCode,
                    ApiKey = Business.FeyinApiKey,
                    Token = Business.FeyinToken
                };
            }
        }

        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    ViewBag.CompanyName = AppData.Name;
        //    base.OnActionExecuted(context);
        //}

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
