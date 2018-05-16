using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace JdCat.Cat.Common
{
    /// <summary>
    /// 待反序列化节点
    /// </summary>
    public class Host
    {
        /// <summary>
        /// appsettings.json字典
        /// </summary>
        public Dictionary<string, Endpoint> Endpoints { get; set; }

        /// <summary>
        /// 配置Kestrel
        /// </summary>
        /// <param name="options"></param>
        public static void SetHost(KestrelServerOptions options)
        {
            var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
            var host = configuration.GetSection("RafHost").Get<Host>();
            foreach (var endpointKvp in host.Endpoints)
            {
                var endpoint = endpointKvp.Value;
                if (!endpoint.IsEnabled)
                {
                    continue;
                }

                var address = IPAddress.Parse(endpoint.Address);
                options.Listen(address, endpoint.Port, opt =>
                {
                    if (endpoint.Certificate == null) return;
                    switch (endpoint.Certificate.Source)
                    {
                        case "File":
                            opt.UseHttps(Path.Combine(Directory.GetCurrentDirectory(), endpoint.Certificate.Path), endpoint.Certificate.Password);
                            break;
                        default:
                            throw new NotImplementedException($"文件 {endpoint.Certificate.Source}还没有实现");
                    }
                });
                options.UseSystemd();
            }
        }
    }

    /// <summary>
    /// 终结点
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 证书
        /// </summary>
        public Certificate Certificate { get; set; }
    }

    /// <summary>
    /// 证书类
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// 源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 证书文件路径，相对于网站根目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 证书密钥
        /// </summary>
        public string Password { get; set; }
    }
}
