using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.FileService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.FileService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UploadController : BaseController
    {

        [HttpPost("Logo")]
        public string Logo([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            return image.SaveLogo(hosting);
        }

        [HttpPost("License")]
        public string License([FromBody]ProductImage image, [FromServices]IHostingEnvironment hosting)
        {
            try
            {
                var name = image.SaveLicense(hosting);
                //UtilHelper.Log(name);
                return name;
            }
            catch (Exception e)
            {
                UtilHelper.Log(e.ToString());
                throw;
            }
            
        }

    }
}