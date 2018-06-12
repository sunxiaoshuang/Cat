using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 订单商品与商品属性关系表，用来配置多对多关系
    /// </summary>
    [Table("OrderProductAttribute", Schema = "dbo")]
    public class OrderProductAttribute : BaseEntity
    {
        public int OrderProductId { get; set; }
        /// <summary>
        /// 订单商品实体
        /// </summary>
        public virtual OrderProduct OrderProduct { get; set; }
        public int AttributeId { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        public virtual ProductAttribute Attribute { get; set; }
    }
}
