using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Filter;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JdCat.Cat.MpApi
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
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<CatDbContext>(a => a.UseMySql(Configuration.GetConnectionString("CatContext"), b => b.MigrationsAssembly("JdCat.Cat.Model")));
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISessionDataRepository, SessionDataRepository>();
            services.AddScoped<IMpRepository, MpRepository>();
            // 跨域
            var urls = Configuration["Cores"].Split(',');
            services.AddCors(options =>
            options.AddPolicy("AllowSameDomain", builder => builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials())
            );

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseCors("AllowSameDomain");
        }
    }
}
