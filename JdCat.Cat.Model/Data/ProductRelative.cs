using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 商品表
    /// </summary>
    [Table("ProductRelative")]
    public class ProductRelative : BaseEntity
    {
        /// <summary>
        /// 套餐商品id
        /// </summary>
        public int SetMealId { get; set; }
        /// <summary>
        /// 套餐商品关联的子商品id
        /// </summary>
        public int ProductId { get; set; }
    }
}
