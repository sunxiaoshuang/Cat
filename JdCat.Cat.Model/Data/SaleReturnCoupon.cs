﻿using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 消费返券
    /// </summary>
    [Table("SaleReturnCoupon")]
    public class SaleReturnCoupon : BaseEntity
    {
        public string Name { get; set; }
        /// <summary>
        /// 价值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 发放数量，-1代表没有限制
        /// </summary>
        public int Quantity { get; set; } = -1;
        /// <summary>
        /// 还剩下的数量，-1代表不限制
        /// </summary>
        public int Stock { get; set; } = -1;
        /// <summary>
        /// 消费金额
        /// </summary>
        public double CostAmount { get; set; }
        /// <summary>
        /// 最低消费，-1代表不需要最低消费
        /// </summary>
        public double MinConsume { get; set; } = -1;
        /// <summary>
        /// 优惠券有效期开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 优惠券有效期结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 优惠券有效天数
        /// </summary>
        public int? ValidDay { get; set; }
        /// <summary>
        /// 已领取的数量
        /// </summary>
        public int Received { get; set; }
        /// <summary>
        /// 已核销的数量
        /// </summary>
        public int Consumed { get; set; }
        /// <summary>
        /// 是否处于激活状态
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDelete { get; set; }
        //private string _status;
        /// <summary>
        /// 优惠券状态说明
        /// </summary>
        [NotMapped]
        public CouponStatus Status
        {
            get
            {
                if (!IsActive)
                {
                    return CouponStatus.Down;
                }
                if (Stock == 0)
                {
                    return CouponStatus.Over;
                }
                var now = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                if (now < StartDate)
                {
                    return CouponStatus.NotBegin;
                }
                if (now > EndDate)
                {
                    return CouponStatus.Expire;
                }
                return CouponStatus.Up;
            }
        }
        /// <summary>
        /// 优惠券所属商户
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }

    }
}
