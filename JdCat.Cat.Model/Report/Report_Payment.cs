using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_Payment
    {
        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号")]
        public int Index { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        [Description("序号")]
        public int Id { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [Description("支付方式")]
        public string Name { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        [Description("订单数")]
        public int Quantity { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [Description("总金额")]
        public double Amount { get; set; }
    }
}
