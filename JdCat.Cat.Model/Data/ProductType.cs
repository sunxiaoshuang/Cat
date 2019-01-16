using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 商品类别表
    /// </summary>
    [Table("ProductType")]
    public class ProductType : BaseEntity
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 所属分类产品
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
        /// <summary>
        /// 分类所属商家id
        /// </summary>
        [Required]
        public int BusinessId { get; set; }
        /// <summary>
        /// 分类所属商家对象
        /// </summary>
        public virtual Business Business { get; set; }

    }
}
