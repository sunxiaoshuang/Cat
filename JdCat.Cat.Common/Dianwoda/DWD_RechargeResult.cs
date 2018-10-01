using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    public class DWD_RechargeResult
    {
        /// <summary>
        /// 是否已充值
        /// </summary>
        public bool rechange_succ { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public string pay_time { get; set; }
        /// <summary>
        /// 入账时间
        /// </summary>
        public string operate_time { get; set; }
    }
}
