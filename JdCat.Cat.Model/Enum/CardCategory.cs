using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 卡券类型
    /// </summary>
    public enum CardCategory
    {
        /// <summary>
        /// 会员卡
        /// </summary>
        [Description("会员卡")]
        MemberCard = 1,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("优惠券")]
        Coupon = 2
    }
}
