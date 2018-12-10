using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    public class WxToken
    {
        /// <summary>
        /// 小程序访问Token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// Token过期时间间隔
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 返回错误码，为空表示请求成功
        /// </summary>
        public int? errcode { get; set; }
        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// Token获取时间
        /// </summary>
        public DateTime? GetTime { get; set; }
        /// <summary>
        /// Token是否过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpires()
        {
            var second = (DateTime.Now - GetTime.Value).TotalSeconds;
            // 如果Token没有过期，则直接返回
            if (second < expires_in - 360)
            {
                return false;
            }
            return true;
        }
    }
}
