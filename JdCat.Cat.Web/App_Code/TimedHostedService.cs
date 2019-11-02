using JdCat.Cat.IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Common;

namespace JdCat.Cat.Web.App_Code
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        public IServiceProvider Service { get; set; }
        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider services)
        {
            _logger = logger;
            Service = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("后台定时任务已启动...");

            _timer = new Timer(TaskElemeToken, null, TimeSpan.Zero, TimeSpan.FromSeconds(18000));       // 五小时执行一次

            return Task.CompletedTask;
        }

        /// <summary>
        /// 定时检查各商户的饿了么token是否过去
        /// </summary>
        /// <param name="state"></param>
        private async void TaskElemeToken(object state)
        {
            using (var scope = Service.CreateScope())
            {
                var rep = scope.ServiceProvider.GetRequiredService<IThirdOrderRepository>();
                var all = await rep.GetAllAsync<Business>();
                var businesses = all.GroupBy(a => new { a.Eleme_AppKey, a.Eleme_AppSecret }).Select(a => a.Key);
                var appData = scope.ServiceProvider.GetRequiredService<AppData>();
                foreach (var business in businesses)
                {
                    if (string.IsNullOrEmpty(business.Eleme_AppKey) || string.IsNullOrEmpty(business.Eleme_AppSecret)) continue;
                    await rep.GetElemeTokenAsync(appData.ElemeToken, business.Eleme_AppKey, business.Eleme_AppSecret);
                }
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("后台定时任务已关闭...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }



    }
}
