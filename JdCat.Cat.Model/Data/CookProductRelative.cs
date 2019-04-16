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
    [Table("CookProductRelative")]
    public class CookProductRelative : BaseEntity
    {
        /// <summary>
        /// 员工id
        /// </summary>
        public int StaffId { get; set; }
        /// <summary>
        /// 员工实体
        /// </summary>
        public virtual Staff Staff { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品实体
        /// </summary>
        public virtual Product Product { get; set; }

    }
}
