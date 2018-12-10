using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace JdCat.Cat.Common.Filter
{
    public class GlobalExceptionAttribute : IExceptionFilter
    {
        readonly IHostingEnvironment _env;
        private static readonly ILog log = LogManager.GetLogger(AppSetting.LogRepository.Name, "GlobalException");

        public GlobalExceptionAttribute(IHostingEnvironment env)
        {
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            if (_env.IsDevelopment()) return;
            //var filePath = Path.Combine(_env.ContentRootPath, "Log", "Error", DateTime.Now.ToString("yyyyMMdd") + ".txt");
            //using (var stream = File.AppendText(filePath))
            //{
            //    stream.WriteLine($"{Environment.NewLine}\r\n【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】服务器异常：{context.Exception}");
            //}
            log.Error("服务器异常：" + context.Exception);

            var json = new ErrorResponse(context.Exception.Message) { DeveloperMessage = context.Exception };

            context.Result = new ApplicationErrorResult(json);
            context.ExceptionHandled = true;
        }
    }

    public class ApplicationErrorResult : ObjectResult
    {
        public ApplicationErrorResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
    public class ErrorResponse
    {
        public ErrorResponse(string msg)
        {
            Message = msg;
        }
        public string Message { get; set; }
        public object DeveloperMessage { get; set; }
    }
}
