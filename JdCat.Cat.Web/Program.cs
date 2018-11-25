using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Model;
using JdCat.Cat.Web.App_Code;
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
            Task.Run(async () => await Reset(host));
            host.Run();
        }
        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
                .Build();
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                //                .UseKestrel(Host.SetHost)
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
                    //SyncProgram(context);
                }
                catch (Exception ex)
                {
                    throw new Exception("初始化种子错误", ex);
                }
            }
        }

        /// <summary>
        /// 系统更改后的同步方法
        /// </summary>
        private static void SyncProgram(CatDbContext context)
        {
            // 为每个商户生成ObjectId
            var businesses = context.Businesses.ToList();
            foreach (var item in businesses)
            {
                if (!string.IsNullOrEmpty(item.ObjectId)) continue;
                item.ObjectId = Guid.NewGuid().ToString().ToLower();
            }
            context.SaveChanges();
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
        /// <summary>
        /// 重置城市编码库
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private async static Task Reset(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var tool = new InitTool(host);
                    await tool.ResetCityAsync();
                    await tool.ResetCancelReasonAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("重置工具失败", ex);
                }
            }
        }

    }

}
