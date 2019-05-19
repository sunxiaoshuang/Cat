using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_SaleStatistics
    {
        [Description("序号")]
        public int Index { get; set; }
        [Description("销售日期")]
        public string Date { get; set; }
        [Description("订单实数")]
        public int Quantity { get; set; }
        [Description("商品销售总额")]
        public double ProductOriginalAmount { get; set; }
        [Description("商品销售净额")]
        public double ProductAmount { get; set; }
        [Description("配送费")]
        public double FreightAmount { get; set; }
        [Description("餐盒费")]
        public double PackageAmount { get; set; }
        [Description("商品折扣优惠")]
        public double DiscountAmount { get; set; }
        [Description("活动优惠")]
        public double ActivityAmount { get; set; }
        [Description("优惠总额")]
        public double BenefitAmount { get; set; }
        [Description("应收金额")]
        public double Total { get; set; }
        [Description("实收金额")]
        public double ActualTotal { get; set; }
    }
}
