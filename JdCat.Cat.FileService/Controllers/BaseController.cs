using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace JdCat.Cat.FileService
{
    public class BaseController : Controller 
    {

        private static readonly ILog log = LogManager.GetLogger(AppSetting.LogRepository.Name, "Base");
        public ILog Log
        {
            get
            {
                return log;
            }
        }

    }
}
