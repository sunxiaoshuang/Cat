using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 调用接口返回的结果对象
    /// </summary>
    public class DWD_Result<T> where T : class, new()
    {
        /// <summary>
        /// 接口调用结果状态码
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 接口调用结果
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 接口调用结果信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 接口调用结果对象
        /// </summary>
        public T result { get; set; }
    }
}
