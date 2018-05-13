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
    }
}
