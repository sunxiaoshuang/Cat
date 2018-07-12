using JdCat.Cat.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 微信调起支付API签名列表类
    /// </summary>
    public class WxPayment
    {
        [JsonIgnore]
        public string appId { get; set; }
        [JsonIgnore]
        public string key { get; set; }
        public string timeStamp { get; set; } = UtilHelper.GetTimestamp(DateTime.Now).ToString();
        public string nonceStr { get; set; } = Guid.NewGuid().ToString().Substring(0, 30);
        public string package { get; set; }
        public string signType { get; set; } = "MD5";
        public string paySign { get; set; }

        public void Generator()
        {
            var result = new StringBuilder();
            result.Append("appId=" + appId);
            result.Append("&nonceStr=" + nonceStr);
            result.Append("&package=" + package);
            result.Append("&signType=" + signType);
            result.Append("&timeStamp=" + timeStamp);
            result.Append("&key=" + key);
            paySign = UtilHelper.MD5Encrypt(result.ToString()).ToUpper();
        }
    }

}
