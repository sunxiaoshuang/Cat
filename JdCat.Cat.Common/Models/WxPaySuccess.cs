using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 微信统一支付后返回结果类
    /// </summary>
    public class WxPaySuccess
    {
        public string appid { get; set; }
        public string attach { get; set; }
        public string bank_type { get; set; }
        public string cash_fee { get; set; }
        public string device_info { get; set; }
        public string fee_type { get; set; }
        public string is_subscribe { get; set; }
        public string mch_id { get; set; }
        public string nonce_str { get; set; }
        public string openid { get; set; }
        public string out_trade_no { get; set; }
        public string result_code { get; set; }
        public string return_code { get; set; }
        public string sign { get; set; }
        public string sub_appid { get; set; }
        public string sub_is_subscribe { get; set; }
        public string sub_mch_id { get; set; }
        public string sub_openid { get; set; }
        public string time_end { get; set; }
        public string total_fee { get; set; }
        public string trade_type { get; set; }
        public string transaction_id { get; set; }

    }

}
