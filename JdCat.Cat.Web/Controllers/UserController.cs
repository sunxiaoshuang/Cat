using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class UserController : BaseController<IUserRepository, User>
    {
        public UserController(AppData appData, IUserRepository service) : base(appData, service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult GetList(int pageIndex = 1, int pageSize = 10)
        {
            var query = Service.GetUsers(Business);
            var count = query.Count();
            var list = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return Json(new {
                rows = count,
                list
            });
        }



    }
}
