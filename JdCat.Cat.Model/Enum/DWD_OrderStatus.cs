using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    public enum DWD_OrderStatus
    {
        /// <summary>
        /// 派单中
        /// </summary>
        Assigning = 0,
        /// <summary>
        /// 已转单
        /// </summary>
        Transfer = 3,
        /// <summary>
        /// 取餐中
        /// </summary>
        Taking = 5,
        /// <summary>
        /// 已到店
        /// </summary>
        ArrivedShop = 10,
        /// <summary>
        /// 配送中
        /// </summary>
        Distribution = 15,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 100,
        /// <summary>
        /// 异常
        /// </summary>
        Exception = 98,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 99
    }
}
