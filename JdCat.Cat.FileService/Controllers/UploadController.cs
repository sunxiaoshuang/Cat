using System;
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
    public class UploadController : Controller
    {
        [HttpPost("Logo")]
        public string Logo([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            return image.SaveLogo(hosting);
        }

        [HttpPost("License")]
        public string License([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            return image.SaveLicense(hosting);
        }

    }
}