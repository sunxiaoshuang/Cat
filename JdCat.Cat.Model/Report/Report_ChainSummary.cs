using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    /// <summary>
    /// 连锁店营业汇总类
    /// </summary>
    public class Report_ChainSummary
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public double Amount { get; set; }
    }
}
