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
        /// 收银台
        /// </summary>
        [Description("收银台")]
        Cashier = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        [Description("扫码点餐")]
        Scan = 2
    }
}
