using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 送货方式
    /// </summary>
    public enum DeliveryMode
    {
        /// <summary>
        /// 第三方平台
        /// </summary>
        Third = 0,
        /// <summary>
        /// 自己配送
        /// </summary>
        Own = 1,
        /// <summary>
        /// 自提
        /// </summary>
        Self = 2
    }
}
