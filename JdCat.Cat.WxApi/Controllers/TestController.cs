using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class TestController : BaseController<IUserRepository, User>
    {
        public TestController(IUserRepository service) : base(service)
        {
        }

        [HttpPost("t1")]
        public IActionResult T1([FromBody]dynamic p, [FromServices]IHostingEnvironment env)
        {
            using (var file = System.IO.File.AppendText(Path.Combine(env.ContentRootPath, "t1.txt")))
            {
                file.WriteLine(p.address);
                file.WriteLine("\r\n");
            }
            return Ok("好的");
        }
    }
}
