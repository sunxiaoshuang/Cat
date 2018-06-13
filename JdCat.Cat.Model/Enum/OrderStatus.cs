using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    [Flags]
    public enum OrderStatus : int
    {
        /// <summary>
        /// 已付款（前：商家待接单，后：已付款），有效订单
        /// </summary>
        Payed = 1,
        /// <summary>
        /// 拒绝接单（前：已拒单，后：已拒单），无效订单
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// 商家已接单（前：商家已接单，后：待配送），有效订单
        /// </summary>
        Receipted = 4,
        /// <summary>
        /// 待取货（前：骑手待取货，后：待配送），有效订单
        /// </summary>
        DistributorReceipt = 8,
        /// <summary>
        /// 配送中（前：骑手配送中，后：配送中），有效订单
        /// </summary>
        Distribution = 16,
        /// <summary>
        /// 配送异常，无效订单
        /// </summary>
        Exception = 32,
        /// <summary>
        /// 已送达（前：待确认收货，后：已送达），有效订单
        /// </summary>
        Achieve = 64,
        /// <summary>
        /// 已完成（前：订单已完成，后：用户已确认收货），有效订单
        /// </summary>
        Finish = 128,
        /// <summary>
        /// 未付款，无效订单
        /// </summary>
        NotPay = 256,
        /// <summary>
        /// 已评价（前：订单已完成，后：已评价），有效订单
        /// </summary>
        Appraised = 512,
        /// <summary>
        /// 已关闭，无效订单
        /// </summary>
        Close = 1024
    }
}
