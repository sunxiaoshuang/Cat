using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单详情
    /// </summary>
    public class DWD_Price
    {
        /// <summary>
        /// 应收商家总费用，单位：分
        /// </summary>
        public long receivable_price { get; set; }
        /// <summary>
        /// 费用类型，[1:预估费用,2:最终费用]
        /// </summary>
        public int price_type { get; set; }
        /// <summary>
        /// 费用明细
        /// </summary>
        public DWD_PriceInfo receivable_info { get; set; }
    }
}
