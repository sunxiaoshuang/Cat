using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 城市名称与编码对应表
    /// </summary>
    [Table("PaymentTarget")]
    public class PaymentTarget : BaseEntity
    {
        /// <summary>
        /// 关联对象id（会员id）
        /// </summary>
        public int ObjectId { get; set; }
        /// <summary>
        /// 支付码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 支付订单id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 支付状态[0:未使用,1:已使用]
        /// </summary>
        public int Status { get; set; }
    }
}
