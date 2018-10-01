using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单详情
    /// </summary>
    public class DWD_PriceInfo
    {
        /// <summary>
        /// 配送费，单位:分
        /// </summary>
        public long delivery_price { get; set; }
        /// <summary>
        /// 小费，单位:分
        /// </summary>
        public long tip_price { get; set; }
        /// <summary>
        /// 恶劣天气加价，单位:分
        /// </summary>
        public long weather_price { get; set; }
        /// <summary>
        /// 短信服务费，单位:分
        /// </summary>
        public long sms_price { get; set; }

    }
}
