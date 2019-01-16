using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Filter;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Repository;
using JdCat.Cat.Repository.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi
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
            //services.AddDbContext<CatDbContext>(a => a.UseLazyLoadingProxies()
            //.ConfigureWarnings(b => b.Log(CoreEventId.DetachedLazyLoadingWarning))
            //.UseSqlServer(Configuration.GetConnectionString("CatContext"), b => b.MigrationsAssembly("JdCat.Cat.Model")));
            //services.AddDbContext<CatDbContext>(a => a.UseSqlServer(Configuration.GetConnectionString("CatContext"), b => b.MigrationsAssembly("JdCat.Cat.Model")));
            services.AddDbContext<CatDbContext>(a => a.UseMySql(Configuration.GetConnectionString("CatContext"), b => b.MigrationsAssembly("JdCat.Cat.Model")));

            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISessionDataRepository, SessionDataRepository>();
            // 系统参数
            var config = new AppData();
            config.Init(Configuration);
            services.AddSingleton(config);
            InputData.Key = config.ServerKey;
            AppSetting.SetAppData(config);
            // 达达请求
            var dada = DadaHelper.GetHelper();
            dada.Init(config, AppData.JsonSetting);
            services.AddSingleton(dada);
            // 点我达请求
            var dwd = DwdHelper.GetHelper();
            dwd.Init(config);
            services.AddSingleton(dwd);
            // 易联云
            var yly = YlyHelper.GetHelper();
            yly.Init(config.YlyPartnerId, config.YlyApiKey, config.YlyUrl);
            services.AddSingleton(yly);
            // 飞鹅
            var feie = FeieHelper.GetHelper();
            feie.Init(config.FeieUser, config.FeieKey, config.FeieUrl);
            services.AddSingleton(yly);
            // 外卖管家
            var wmgj = WmgjHelper.GetHelper();
            wmgj.Init(int.Parse(config.WmgjAppId), config.WmgjAppKey, config.WmgjUrl);
            services.AddSingleton(yly);
            // 一城飞客
            services.AddSingleton(YcfkHelper.GetHelper().Init(config));
            // 微信
            WxHelper.Init(config);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseMvc();
        }
    }
}
