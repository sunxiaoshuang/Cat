
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model
{
    /// <summary>
    /// 达达接口返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DadaResult<T> where T : class, new()
    {
        /// <summary>
        /// 状态值：success|fail
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public int? errorCode { get; set; }
        /// <summary>
        /// 结果码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 调用成功时返回的结果
        /// </summary>
        public T result { get; set; }
        /// <summary>
        /// 接口调用是否成功
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return status == "success";
        }
    }
}
