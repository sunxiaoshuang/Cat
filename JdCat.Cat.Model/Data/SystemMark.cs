using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 系统备注
    /// </summary>
    [Table("SystemMark")]
    public class SystemMark : BaseEntity
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标签类型
        /// </summary>
        public MarkCategory Category { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
