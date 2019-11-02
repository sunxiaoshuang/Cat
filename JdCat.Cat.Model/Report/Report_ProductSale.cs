using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_ProductSale
    {
        [Description("序号")]
        public int Id { get; set; }
        [Description("名称")]
        public string Name { get; set; }
        [Description("总数")]
        public double Count { get; set; }
        [Description("总销售额")]
        public double Amount { get; set; }
        [Description("商户编号")]
        public int BusinessId { get; set; }
    }
}
