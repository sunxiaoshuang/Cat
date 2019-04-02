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
    public class PaymentType : BaseEntityClient
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 支付编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 显示图标
        /// </summary>
        public SystemIcon Icon { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public PaymentTypeStatus TypeStatus { get; set; }
    }
}
