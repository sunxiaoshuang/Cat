using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 订单退款状态
    /// </summary>
    public enum OrderRefundStatus
    {
        /// <summary>
        /// 正常订单，未执行退款
        /// </summary>
        None = 0,
        /// <summary>
        /// 正在申请退款
        /// </summary>
        Apply = 1,
        /// <summary>
        /// 已退款
        /// </summary>
        Finish = 2
    }
}
