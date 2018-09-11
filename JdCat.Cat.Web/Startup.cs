using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Filter;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository;
using JdCat.Cat.Web.App_Code;
using JdCat.Cat.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JdCat.Cat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                // 注册全局异常过滤器
                options.Filters.Add<GlobalExceptionAttribute>();
            });
            // 配置会话应用状态
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.HttpOnly = true;
            });
            // 配置依赖
            services.AddDbContext<CatDbContext>(a => a.UseSqlServer(Configuration.GetConnectionString("CatContext"), b => b.MigrationsAssembly("JdCat.Cat.Model")));
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDwdRepository, DwdRepository>();
            services.AddSingleton(new List<City>());
            services.AddSingleton(new List<DadaCancelReason>());
            // 系统参数
            var appData = Configuration.GetSection("appData");
            var config = new AppData
            {
                ApiUri = appData["apiUri"],
                FileUri = appData["fileUri"],
                Name = appData["name"],
                Session = appData["session"],
                Cookie = appData["cookie"],
                Connection = Configuration.GetConnectionString("CatContext"),
                OrderUrl = appData["orderUrl"],
                RunMode = appData["runMode"],
                DadaDomain = appData["dadaDomain"],
                DadaAppKey = appData["dadaAppKey"],
                DadaAppSecret = appData["dadaAppSecret"],
                DadaSourceId = appData["dadaSourceId"],
                DadaShopNo = appData["dadaShopNo"],
                DadaCallback = appData["dadaCallback"],
                FeyinAppId = appData["feyinAppId"],
                FeyinMemberCode = appData["feyinMemberCode"],
                FeyinApiKey = appData["feyinApiKey"],
                DwdDomain = appData["dwdDomain"],
                DwdAppKey = appData["dwdAppKey"],
                DwdAppSecret = appData["dwdAppSecret"],
                DwdShopNo = appData["dwdShopNo"],
                DwdCallback = appData["dwdCallback"]
            };
            services.AddSingleton(config);
            // 序列化参数
            var setting = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            services.AddSingleton(setting);
            // 达达请求
            services.AddSingleton(new DadaHelper(config, setting));
            // 点我达请求
            services.AddSingleton(new DwdHelper(config));
            // 飞印
            var feyin = new FeYinHelper
            {
                AppId = config.FeyinAppId,
                MemberCode = config.FeyinMemberCode,
                ApiKey = config.FeyinApiKey
            };
            services.AddSingleton(feyin);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
