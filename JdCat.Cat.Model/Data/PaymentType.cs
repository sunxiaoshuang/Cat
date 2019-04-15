using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 支付方式
    /// </summary>
    [Table("PaymentType")]
    public class PaymentType : BaseEntity
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 支付类别
        /// </summary>
        public PaymentCategory Category { get; set; }
        /// <summary>
        /// 对象状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
