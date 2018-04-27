using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Business")]
    public class BusinessController
    {
        private readonly IBusinessRepository service;
        private readonly UtilHelper helper;
        public BusinessController(IBusinessRepository service, UtilHelper helper)
        {
            this.service = service;
            this.helper = helper;
        }
        //[HttpGet]
        //public IEnumerable<Business> Get()
        //{
        //    var list = service.GetAll().ToList();
        //    if (!list.Any())
        //    {
        //        var business = new Business
        //        {
        //            Code = "sunxsh", Name = "孙小双", Password = helper.GetMd5("000000"), RegisterDate = DateTime.Now
        //        };
        //        service.Add(business);
        //        list.Add(business);
        //    }
        //    return list;
        //}

        public Business GetBusiness(string user, string pwd)
        {
            var password = helper.GetMd5(pwd);
            var entity = service.GetAll().FirstOrDefault(a => (a.Code == user || a.Mobile == user) && a.Password == password);
            return entity;
        }

    }
}