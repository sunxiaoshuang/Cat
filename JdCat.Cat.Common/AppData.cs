using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common
{
    public class AppData
    {
        /// <summary>
        /// API请求的地址
        /// </summary>
        public string ApiUri { get; set; }
        /// <summary>
        /// 文件服务器
        /// </summary>
        public string FileUri { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户信息存储Session名称
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        /// 存储用户Cookie的名称
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 新订单通知接口
        /// </summary>
        public string OrderUrl { get; set; }
        /// <summary>
        /// 达达接口域名
        /// </summary>
        public string DadaDomain { get; set; }
        /// <summary>
        /// 简单猫开发者标识
        /// </summary>
        public string DadaAppKey { get; set; }
        /// <summary>
        /// 简单猫开发者密码
        /// </summary>
        public string DadaAppSecret { get; set; }
        /// <summary>
        /// 绑定到开发者账号中的某个商户编号
        /// </summary>
        public string DadaSourceId { get; set; }
    }
}
