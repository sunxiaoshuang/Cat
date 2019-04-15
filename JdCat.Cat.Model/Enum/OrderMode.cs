using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 活动状态
    /// </summary>
    public enum OrderMode
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// 快餐
        /// </summary>
        [Description("快餐")]
        FastFood = 1,
        /// <summary>
        /// 中餐
        /// </summary>
        [Description("中餐")]
        ChineseFood = 2
    }
}
