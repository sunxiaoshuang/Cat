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
        Cashier = 1,
        /// <summary>
        /// 扫码点餐
        /// </summary>
        Scan = 2
    }
}
