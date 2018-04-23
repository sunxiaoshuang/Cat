using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public BusinessController(IBusinessRepository service)
        {
            this.service = service;
        }
        [HttpGet]
        public IEnumerable<Business> Get()
        {
            var list = service.GetAll().ToList();
            if (!list.Any())
            {
                var business = new Business
                {
                    Code = "sunxsh", Name = "孙小双", Password = "000000", RegisterDate = DateTime.Now
                };
                service.Add(business);
                list.Add(business);
            }
            return list;
        }
    }
}