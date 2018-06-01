using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.WxApi.Models
{
    public class WxRetInfo
    {
        /// <summary>
        /// 状态码，成功则返回0
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// 登录成功后的用户信息
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 错误提示
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }

    public class WxUserData
    {
        public int Skey { get; set; }
        public User Userinfo { get; set; }
    }
}
