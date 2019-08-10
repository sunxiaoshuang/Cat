using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 充值记录
    /// </summary>
    [Table("ChargeRecord")]
    public class ChargeRecord : BaseEntity
    {
        /// <summary>
        /// 充值金额（单位：分）
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 赠送金额（单位：分）
        /// </summary>
        public int Give { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Bonus { get; set; }

        /// <summary>
        /// 充值编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 支付标识码
        /// </summary>
        public string PrepayId { get; set; }

        /// <summary>
        /// 用户公众号唯一标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string WxPayCode { get; set; }

        /// <summary>
        /// 支付状态[0:未支付，1:已支付]
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 关联的成员id
        /// </summary>
        public int RelativeId { get; set; }

        /// <summary>
        /// 充值的商户id
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }

    }
}
