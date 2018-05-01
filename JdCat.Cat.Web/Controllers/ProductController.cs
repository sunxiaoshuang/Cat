using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(AppData appData) : base(appData)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddType([FromServices]IProductRepository service)
        {
            var types = service.GetTypes(Business);
            return PartialView(types);
        }

        [HttpPost]
        public IEnumerable<ProductType> UpdateTypes(IEnumerable<ProductType> add, IEnumerable<ProductType> edit, IEnumerable<int> remove, [FromServices]IProductRepository service)
        {
            if(add.Count() > 0)
            {
                foreach (var item in add)
                {
                    item.BusinessId = Business.ID;
                }
                service.AddTypes(add);
            }
            if (edit.Count() > 0)
            {
                service.EditTypes(edit);
            }
            if (remove.Count() > 0)
            {
                service.RemoveTypes(remove);
            }
            service.Commit();



            return null;
        }
    }
}