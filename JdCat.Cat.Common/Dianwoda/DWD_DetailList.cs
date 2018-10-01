using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单回调信息对象
    /// </summary>
    public class DWD_DetailList
    {
        /// <summary>
        /// 交易编号
        /// </summary>
        public string serial_id { get; set; }
        /// <summary>
        /// 入账时间
        /// </summary>
        public string operate_time { get; set; }
        /// <summary>
        /// 收支金额(分)
        /// </summary>
        public long amount { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 交易内容
        /// </summary>
        public string trade_content { get; set; }
    }
}
