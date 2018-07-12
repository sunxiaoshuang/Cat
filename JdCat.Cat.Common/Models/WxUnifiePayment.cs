using JdCat.Cat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 微信统一支付接口参数类
    /// </summary>
    [XmlRoot("xml")]
    public class WxUnifiePayment
    {
        private static string ip;
        static WxUnifiePayment()
        {

            //var list = Dns.GetHostAddresses(Environment.MachineName);
            //foreach (var item in list)
            //{
            //    Dns.addre
            //}

            ip = "203.195.205.143";
        }
        /// <summary>
        /// 服务号appid
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; } = "简单猫科技";
        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; } = "简单猫科技-订单支付";
        /// <summary>
        /// 商品详情，可为空
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// 设备号，可以为空
        /// </summary>
        public string device_info { get; set; } = "jiandanmao";
        /// <summary>
        /// 币种
        /// </summary>
        public string fee_type { get; set; } = "CNY";
        /// <summary>
        /// 订单优惠标记
        /// </summary>
        public string goods_tag { get; set; }
        /// <summary>
        /// 指定支付方式，指定no_credit，则不能用信用卡支付
        /// </summary>
        public string limit_pay { get; set; }
        /// <summary>
        /// 服务商商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; } = Guid.NewGuid().ToString().Substring(0, 30);
        /// <summary>
        /// 通知地址
        /// </summary>
        public string notify_url { get; set; } = "https://www.jiandanmao.cn/api/order/paySuccess";
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string product_id { get; set; }
        /// <summary>
        /// 签名，有必要的签名算法
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 签名类型
        /// </summary>
        public string sign_type { get; set; } = "MD5";
        /// <summary>
        /// 终端IP
        /// </summary>
        public string spbill_create_ip { get; set; } = ip;
        /// <summary>
        /// 最终支付的小程序appid
        /// </summary>
        public string sub_appid { get; set; }
        /// <summary>
        /// 服务商商户号下的子商户号
        /// </summary>
        public string sub_mch_id { get; set; }
        /// <summary>
        /// 子用户标识
        /// </summary>
        public string sub_openid { get; set; }
        /// <summary>
        /// 交易结束时间
        /// </summary>
        public string time_expire { get; set; }
        /// <summary>
        /// 交易起始时间:20180201112231
        /// </summary>
        public string time_start { get; set; }
        /// <summary>
        /// 标价金额，单位为分
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; } = "JSAPI";
        /// <summary>
        /// 服务商商户号密钥
        /// </summary>
        [XmlIgnore]
        public string key { get; set; }

        public void Generator()
        {
            var result = new StringBuilder();
            result.Append("appid=" + appid);
            if(!string.IsNullOrEmpty(attach)) result.Append("&attach=" + attach);
            if(!string.IsNullOrEmpty(body)) result.Append("&body=" + body);
            if(!string.IsNullOrEmpty(detail)) result.Append("&detail=" + detail);
            if(!string.IsNullOrEmpty(device_info)) result.Append("&device_info=" + device_info);
            if(!string.IsNullOrEmpty(fee_type)) result.Append("&fee_type=" + fee_type);
            if(!string.IsNullOrEmpty(goods_tag)) result.Append("&goods_tag=" + goods_tag);
            if(!string.IsNullOrEmpty(limit_pay)) result.Append("&limit_pay=" + limit_pay);
            if(!string.IsNullOrEmpty(mch_id)) result.Append("&mch_id=" + mch_id);
            if(!string.IsNullOrEmpty(nonce_str)) result.Append("&nonce_str=" + nonce_str);
            if(!string.IsNullOrEmpty(notify_url)) result.Append("&notify_url=" + notify_url);
            if(!string.IsNullOrEmpty(openid)) result.Append("&openid=" + openid);
            if(!string.IsNullOrEmpty(out_trade_no)) result.Append("&out_trade_no=" + out_trade_no);
            if(!string.IsNullOrEmpty(product_id)) result.Append("&product_id=" + product_id);
            if(!string.IsNullOrEmpty(sign_type)) result.Append("&sign_type=" + sign_type);
            if(!string.IsNullOrEmpty(spbill_create_ip)) result.Append("&spbill_create_ip=" + spbill_create_ip);
            if(!string.IsNullOrEmpty(sub_appid)) result.Append("&sub_appid=" + sub_appid);
            if(!string.IsNullOrEmpty(sub_mch_id)) result.Append("&sub_mch_id=" + sub_mch_id);
            if(!string.IsNullOrEmpty(sub_openid)) result.Append("&sub_openid=" + sub_openid);
            if (!string.IsNullOrEmpty(time_expire)) result.Append("&time_expire=" + time_expire);
            if(!string.IsNullOrEmpty(time_start)) result.Append("&time_start=" + time_start);
            result.Append("&total_fee=" + total_fee);
            result.Append("&trade_type=" + trade_type);
            result.Append("&key=" + key);

            sign = UtilHelper.MD5Encrypt(result.ToString()).ToUpper();
        }
    }
}
