using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 物流类型
    /// </summary>
    public enum LogisticsType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 自己配送
        /// </summary>
        Self = 1,
        /// <summary>
        /// 点我达配送
        /// </summary>
        Dada = 2,
        /// <summary>
        /// 美团配送
        /// </summary>
        Meituan = 3,
        /// <summary>
        /// 蜂鸟配送
        /// </summary>
        Fengniao = 4,
        /// <summary>
        /// 点我达配送
        /// </summary>
        Dianwoda = 5
    }
}
