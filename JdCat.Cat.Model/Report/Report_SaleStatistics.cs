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
        [Description("销售金额")]
        public double Total { get; set; }
        [Description("配送费")]
        public double FreightAmount { get; set; }
        [Description("餐盒费")]
        public double PackageAmount { get; set; }
        [Description("销售数量")]
        public int Quantity { get; set; }
    }
}
