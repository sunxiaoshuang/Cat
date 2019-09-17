using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    [Flags]
    public enum OrderSource
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [Description("未定义")]
        None = 0,
        /// <summary>
        /// 收银台
        /// </summary>
        [Description("收银台")]
        Cashier = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        [Description("扫码点餐")]
        Scan = 2,
        /// <summary>
        /// 小程序
        /// </summary>
        [Description("小程序")]
        SmallProgram = 4,
        /// <summary>
        /// 美团
        /// </summary>
        [Description("美团")]
        Meituan = 8,
        /// <summary>
        /// 饿了么
        /// </summary>
        [Description("饿了么")]
        Eleme = 16
    }
}
