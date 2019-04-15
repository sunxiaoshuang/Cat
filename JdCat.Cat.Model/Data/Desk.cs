using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 餐桌
    /// </summary>
    [Table("Desk")]
    public class Desk : BaseEntity
    {
        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 餐桌人数
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 餐桌实体状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 所属餐桌类别id
        /// </summary>
        public int DeskTypeId { get; set; }
        /// <summary>
        /// 所属餐桌类别对象
        /// </summary>
        public virtual DeskType DeskType { get; set; }
    }
}
