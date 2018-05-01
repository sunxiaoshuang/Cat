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
    [Route("api/product")]
    public class ProductController
    {
        private readonly IProductRepository service;
        public ProductController(IProductRepository service)
        {
            this.service = service;
        }

        [Route("producttype")]
        public IEnumerable<ProductType> PostProductType([FromBody]IEnumerable<ProductType> productTypes)
        {
            service.AddTypes(productTypes);
            return productTypes;
        }

    }
}