using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单商品信息
    /// </summary>
    public class DWD_Product
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 商品单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 商品原价
        /// </summary>
        public long unit_price { get; set; }
        /// <summary>
        /// 商品折扣价
        /// </summary>
        public long discount_price { get; set; }
        /// <summary>
        /// 商品制作时长
        /// </summary>
        public long production_time { get; set; }
    }
}
