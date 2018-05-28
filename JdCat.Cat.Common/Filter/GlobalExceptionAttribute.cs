using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JdCat.Cat.Common.Filter
{
    public class GlobalExceptionAttribute: IExceptionFilter
    {
        readonly IHostingEnvironment _env;

        public GlobalExceptionAttribute(IHostingEnvironment env)
        {
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            if (_env.IsDevelopment()) return;
            var filePath = Path.Combine(_env.ContentRootPath, "Log", "Error", DateTime.Now.ToString("yyyyMMdd") + ".txt");
            using (var stream = File.AppendText(filePath))
            {
                stream.WriteLine(context.Exception.ToString());
            }
        }
    }
}
