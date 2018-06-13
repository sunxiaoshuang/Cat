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
                var util = scope.ServiceProvider.GetService<UtilHelper>();
                var setting = scope.ServiceProvider.GetService<JsonSerializerSettings>();
                var appData = scope.ServiceProvider.GetService<AppData>();
                var cityList = scope.ServiceProvider.GetService<List<City>>();
                using (var hc = new HttpClient())
                {
                    var req = new DadaTrans { Timestamp = util.ConvertDateTimeToInt(DateTime.Now) };
                    req.App_key = appData.DadaAppKey;
                    req.App_secret = appData.DadaAppSecret;
                    req.Source_id = appData.DadaSourceId;
                    req.Generator();
                    req.Signature = util.MD5Encrypt(req.Signature).ToUpper();
                    var p = JsonConvert.SerializeObject(req, setting);
                    var body = new StringContent(p);
                    body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await hc.PostAsync(appData.DadaDomain + "/api/cityCode/list", body);
                    var content = await result.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);
                    var status = json["status"].Value<string>();
                    if(status == "fail")
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Log", "Error", DateTime.Now.ToString("yyyyMMdd") + ".txt");
                        using (var stream = File.AppendText(filePath))
                        {
                            stream.WriteLine($"{Environment.NewLine}\r\n城市编码重置异常[{DateTime.Now:yyyy-MM-dd HH:ss:mm}]：{json["msg"].Value<string>()}：");
                        }
                        cityList = context.Citys.ToList();
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
    }
}
