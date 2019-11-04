using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Filter;
using JdCat.Cat.Common.Weixin;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Repository;
using JdCat.Cat.Repository.Service;
using JdCat.Cat.Web.App_Code;
using JdCat.Cat.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

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
            })
            .AddJsonOptions(a => a.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
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
            // 注册redis连接
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConn")));

            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDwdRepository, DwdRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<ITangRepository, TangRepository>();
            services.AddScoped<IUtilRepository, UtilRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IThirdOrderRepository, ThirdOrderRepository>();
            services.AddSingleton(new List<City>());
            services.AddSingleton(new List<DadaCancelReason>());
            // 系统参数
            var config = new AppData();
            config.Init(Configuration);
            services.AddSingleton(config);
            //InputData.Key = config.ServerKey;
            AppSetting.SetAppData(config);
            // 序列化参数
            services.AddSingleton(AppData.JsonSetting);


            // 达达请求
            var dada = DadaHelper.GetHelper();
            dada.Init(config, AppData.JsonSetting);
            services.AddSingleton(dada);
            // 点我达请求
            var dwd = DwdHelper.GetHelper();
            dwd.Init(config);
            services.AddSingleton(dwd);
            // 飞印
            var feyin = new FeYinHelper
            {
                AppId = config.FeyinAppId,
                MemberCode = config.FeyinMemberCode,
                ApiKey = config.FeyinApiKey
            };
            services.AddSingleton(feyin);
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
            // 微信加解密对象
            services.AddSingleton(new WXBizMsgCrypt(config.OpenToken, config.OpenEncodingAESKey, config.OpenAppId));
            // 一城飞客
            services.AddSingleton(YcfkHelper.GetHelper().Init(config));
            // 微信
            WxHelper.Init(config);

            // 注册定时服务
            if (config.IsTimer)
            {
                services.AddHostedService<TimedHostedService>();
            }
            
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
