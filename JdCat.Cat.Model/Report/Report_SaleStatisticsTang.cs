using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_SaleStatisticsTang
    {
        [Description("序号")]
        public int Index { get; set; }
        [Description("销售日期")]
        public string Date { get; set; }
        [Description("订单实数")]
        public int Quantity { get; set; }
        [Description("商品销售总额")]
        public double GoodAmount { get; set; }
        [Description("商品销售净额")]
        public double ActualGoodAmount { get; set; }
        [Description("餐位费")]
        public double MealFee { get; set; }
        [Description("折扣商品总额")]
        public double GoodDiscountAmount { get; set; }
        [Description("订单折扣优惠")]
        public double OrderDiscountAmount { get; set; }
        [Description("订单优惠金额")]
        public double PreferentialAmount { get; set; }
        [Description("应收金额")]
        public double Amount { get; set; }
        [Description("实收金额")]
        public double ActualAmount { get; set; }
    }
}
