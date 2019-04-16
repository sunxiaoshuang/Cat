using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 岗位权限
    /// </summary>
    [Flags]
    public enum StaffPostAuth
    {
        /// <summary>
        /// 收银
        /// </summary>
        Cash = 1,
        /// <summary>
        /// 管理
        /// </summary>
        Manager = 2,
        /// <summary>
        /// 厨师
        /// </summary>
        Cook = 4,
        /// <summary>
        /// 服务员
        /// </summary>
        Waiter = 8
    }
}
