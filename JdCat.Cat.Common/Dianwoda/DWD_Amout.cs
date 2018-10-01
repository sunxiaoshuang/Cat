using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单回调信息对象
    /// </summary>
    public class DWD_Amout : DWDProperty<DWD_Amout>
    {
        /// <summary>
        /// 商户编号
        /// </summary>
        public string store_id { get; set; }
        /// <summary>
        /// 简单猫业务流水
        /// </summary>
        public string serial_no { get; set; }
        /// <summary>
        /// 充值渠道：alipay，wechat，unionpay
        /// </summary>
        public string rechange_channle { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public long amout { get; set; }
        /// <summary>
        /// 支付完成后跳转页面，需要UrlEncode编码
        /// </summary>
        public string return_url { get; set; }

    }
}
