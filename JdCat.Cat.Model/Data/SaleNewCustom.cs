using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 新客户立减
    /// </summary>
    [Table("SaleNewCustom")]
    public class SaleNewCustom : BaseEntity
    {
        /// <summary>
        /// 立减金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户实体对象
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
