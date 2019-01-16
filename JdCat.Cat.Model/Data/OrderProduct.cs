using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 订单商品表
    /// </summary>
    [Table("OrderProduct")]
    public class OrderProduct : BaseEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public double? Quantity { get; set; }
        /// <summary>
        /// 商品价格（折扣价）
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double? OldPrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public double? Discount { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 描述（规格 + 属性）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
        /// <summary>
        /// 商品特性
        /// </summary>
        public ProductFeature Feature { get; set; }
        /// <summary>
        /// 备用属性
        /// </summary>
        [NotMapped]
        public object Tag1 { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// 商品规格id
        /// </summary>
        public int? FormatId { get; set; }
        public virtual ProductFormat Format { get; set; }
        /// <summary>
        /// 折扣id
        /// </summary>
        public int? SaleProductDiscountId { get; set; }
        public virtual SaleProductDiscount SaleProductDiscount { get;set;}
        /// <summary>
        /// 折扣商品数量
        /// </summary>
        public int? DiscountProductQuantity { get; set; }
    }
}
