using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 新增订单后，达达返回的结果类
    /// </summary>
    [Table("DadaReturn", Schema = "dbo")]
    public class DadaReturn : BaseEntity
    {
        /// <summary>
        /// 配送距离(单位：米)
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// 实际运费(单位：元)，运费减去优惠券费用
        /// </summary>
        public double Fee { get; set; }
        /// <summary>
        /// 运费(单位：元)
        /// </summary>
        public double DeliverFee { get; set; }
        /// <summary>
        /// 优惠券费用(单位：元)
        /// </summary>
        public double? CouponFee { get; set; }
        /// <summary>
        /// 小费(单位：元)
        /// </summary>
        public double? Tips { get; set; }
        /// <summary>
        /// 保价费(单位：元)
        /// </summary>
        public double? InsuranceFee { get; set; }
        /// <summary>
        /// 对应的系统订单Id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
