using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    public class DWD_RechargeCallback
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        public bool store_id { get; set; }
        /// <summary>
        /// 充值流水号
        /// </summary>
        public string serial_no { get; set; }
        /// <summary>
        /// 充值金额，分
        /// </summary>
        public string amout { get; set; }
        /// <summary>
        /// 点我达业务流水号
        /// </summary>
        public string biz_no { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sig { get; set; }
    }
}
