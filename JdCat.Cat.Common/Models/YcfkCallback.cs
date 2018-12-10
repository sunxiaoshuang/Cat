using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 一城飞客订单推送
    /// </summary>
    public class YcfkCallback
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }
    }
}
