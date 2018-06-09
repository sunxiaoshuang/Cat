using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 等待付款
        /// </summary>
        Waiting = 1,
        /// <summary>
        /// 等待商户接单
        /// </summary>
        Receipt = 2,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 3,
        /// <summary>
        /// 配送中
        /// </summary>
        Distribution = 4,
        /// <summary>
        /// 已送达
        /// </summary>
        Achieve = 5,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 6
    }
}
