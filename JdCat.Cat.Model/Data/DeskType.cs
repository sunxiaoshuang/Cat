using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 餐桌分类
    /// </summary>
    [Table("DeskType")]
    public class DeskType : BaseEntity
    {
        /// <summary>
        /// 餐桌类别名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 餐桌类别排序码
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 餐桌类别所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 餐桌类别所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 餐桌类别下的餐桌
        /// </summary>
        public virtual ICollection<Desk> Desks { get; set; }
    }
}
