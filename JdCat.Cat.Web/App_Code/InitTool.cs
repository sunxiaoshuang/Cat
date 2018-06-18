using JdCat.Cat.Common;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using JdCat.Cat.Common.Models;

namespace JdCat.Cat.Web.App_Code
{
    /// <summary>
    /// 初始化工具类
    /// </summary>
    public class InitTool
    {
        private IWebHost host;
        public InitTool(IWebHost host)
        {
            this.host = host;
        }
        /// <summary>
        /// 重置城市编码库
        /// </summary>
        public async Task ResetCityAsync()
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<CatDbContext>();
                var cityList = scope.ServiceProvider.GetService<List<City>>();
                var dadaHelper = scope.ServiceProvider.GetService<DadaHelper>();
                using (var hc = new HttpClient())
                {
                    var content = await dadaHelper.RequestAsync("/api/cityCode/list");
                    var json = JObject.Parse(content);
                    var status = json["status"].Value<string>();
                    if (status == "fail")
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Log", "Error", DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        using (var stream = File.AppendText(filePath))
                        {
                            stream.WriteLine($"{Environment.NewLine}\r\n【{DateTime.Now:yyyy-MM-dd HH:ss:mm}】城市编码重置异常：{json["msg"].Value<string>()}");
                        }
                        cityList.AddRange(context.Citys.AsNoTracking().ToList());
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand("delete dbo.[City]");
                        var list = json["result"].Value<JArray>();
                        foreach (var city in list)
                        {
                            var entity = new City() { Code = city["cityCode"].Value<string>(), Name = city["cityName"].Value<string>() };
                            context.Citys.Add(entity);
                            cityList.Add(entity);
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 重置城市编码库
        /// </summary>
        public async Task ResetCancelReasonAsync()
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<CatDbContext>();
                var reasonList = scope.ServiceProvider.GetService<List<DadaCancelReason>>();
                var dadaHelper = scope.ServiceProvider.GetService<DadaHelper>();
                using (var hc = new HttpClient())
                {
                    var content = await dadaHelper.RequestAsync("/api/order/cancel/reasons");
                    var json = JObject.Parse(content);
                    var status = json["status"].Value<string>();
                    if (status == "fail")
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Log", "Error", DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        using (var stream = File.AppendText(filePath))
                        {
                            stream.WriteLine($"{Environment.NewLine}\r\n【{DateTime.Now:yyyy-MM-dd HH:ss:mm}】达达订单取消原因重置异常：{json["msg"].Value<string>()}");
                        }
                        reasonList.AddRange(context.DadaCancelReasons.AsNoTracking().ToList());
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand("delete dbo.[DadaCancelReason]");
                        var list = json["result"].Value<JArray>();
                        foreach (var reason in list)
                        {
                            var entity = new DadaCancelReason() { FlagId = reason["id"].Value<int>(), Reason = reason["reason"].Value<string>() };
                            context.DadaCancelReasons.Add(entity);
                            reasonList.Add(entity);
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

    }
}
