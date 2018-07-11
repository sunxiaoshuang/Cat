using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.WxApi.Models
{
    /// <summary>
    /// 微信统一支付后返回结果类
    /// </summary>
    public class WxUnifieResult
    {
        /// <summary>
        /// 状态码 SUCCESS/FAIL，SUCCESS时为请求成功
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string return_msg { get; set; }

        #region return_code为SUCCESS时返回以下结果
        /// <summary>
        /// 调用接口提交的小程序id
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 微信返回的随机码
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        #endregion

        #region return_code与result_code均返回SUCCESS时返回以下结果
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 预支付会话标识，有效期24小时
        /// </summary>
        public string prepay_id { get; set; }
        /// <summary>
        /// 二维码链接
        /// </summary>
        public string code_url { get; set; }
        #endregion
    }

}
