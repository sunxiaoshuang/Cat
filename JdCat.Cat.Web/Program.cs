﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JdCat.Cat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            AutoMigration(host);
            InitSeed(host);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            //            var config = new ConfigurationBuilder()
            //                .SetBasePath(Directory.GetCurrentDirectory())
            //                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //                .Build();
            return WebHost.CreateDefaultBuilder(args)
                //                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseKestrel(Host.SetHost)
                //                .UseKestrel(options =>
                //                {
                //                    options.Listen(IPAddress.Any, 5443, listenOption =>
                //                    {
                //                        var file = Directory.GetCurrentDirectory() + "\\www.jiandanmao.cn.pfx";
                //                        listenOption.UseHttps(file, "0u4t4020v87tri8");
                //                    });
                //                })
                .Build();
        }

        /// <summary>
        /// 初始化种子
        /// </summary>
        private static void InitSeed(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetService<CatDbContext>();
                    new SeedData(context).Seed();
                }
                catch (Exception ex)
                {
                    throw new Exception("初始化种子错误", ex);
                }
            }
        }
        /// <summary>
        /// 自动迁移
        /// </summary>
        /// <param name="host"></param>
        private static void AutoMigration(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetService<CatDbContext>();
                    context.Database.Migrate();

                }
                catch (Exception ex)
                {
                    throw new Exception("数据库自动迁移失败", ex);
                }
            }
        }


    }

}
