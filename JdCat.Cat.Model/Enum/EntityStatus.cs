using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    [Flags]
    public enum EntityStatus
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 2,
        /// <summary>
        /// 所有状态
        /// </summary>
        All = 1 + 2
    }
}
