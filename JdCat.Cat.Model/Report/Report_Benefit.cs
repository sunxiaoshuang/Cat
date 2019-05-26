using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_Benefit
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 优惠方式
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 优惠订单数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 优惠总金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 优惠前订单总额
        /// </summary>
        public double OrderAmount { get; set; }
    }
}
