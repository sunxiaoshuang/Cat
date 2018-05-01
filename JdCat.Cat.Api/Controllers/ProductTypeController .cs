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
    [Route("api/ProductType")]
    public class ProductTypeController
    {
        private readonly IProductRepository service;
        public ProductTypeController(IProductRepository service)
        {
            this.service = service;
        }

        [HttpPost]
        public ProductType Post([FromBody]ProductType type)
        {
            //service.AddTypes(type);
            return type;
        }

    }
}