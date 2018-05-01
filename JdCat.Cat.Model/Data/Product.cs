using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("Product", Schema = "dbo")]
    public class Product : BaseEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商家id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 商家对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public virtual ICollection<ProductImage> Images { get; set; }
        /// <summary>
        /// 产品分类id
        /// </summary>
        public int? ProductTypeId { get; set; }
        /// <summary>
        /// 产品分类对象
        /// </summary>
        public virtual ProductType ProductType { get; set; }
    }
}
