﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.FileService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.FileService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LogoController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            image.SaveLogo(hosting);
            return Ok("保存成功");
        }
    }
}