using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Enum
{
    /// <summary>
    /// 评论等级
    /// </summary>
    [Flags]
    public enum CommentLevel
    {
        /// <summary>
        /// 未定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 很差
        /// </summary>
        Bad = 1,
        /// <summary>
        /// 一般
        /// </summary>
        Commonly = 2,
        /// <summary>
        /// 满意
        /// </summary>
        Normal = 4,
        /// <summary>
        /// 非常满意
        /// </summary>
        VerySatisfied = 8,
        /// <summary>
        /// 无可挑剔
        /// </summary>
        Perfect = 16
    }
}
