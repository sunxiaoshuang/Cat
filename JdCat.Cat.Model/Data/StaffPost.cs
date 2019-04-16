using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 员工岗位
    /// </summary>
    [Table("StaffPost")]
    public class StaffPost : BaseEntity
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 岗位拥有权限
        /// </summary>
        public StaffPostAuth Authority { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 岗位所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 岗位所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 岗位下的员工
        /// </summary>
        public virtual ICollection<Staff> Staffs { get; set; }
    }
}
