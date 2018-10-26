using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 购物车
    /// </summary>
    [Table("ShoppingCart", Schema = "dbo")]
    public class ShoppingCart : BaseEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 购物车图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 所选规格包装盒数量
        /// </summary>
        public int PackingQuantity { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// 购物车商品规格id
        /// </summary>
        public int FormatId { get; set; }
        public virtual ProductFormat Format { get; set; }
        /// <summary>
        /// 购物车备注描述（规格、属性）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        public virtual User User { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public int? BusinessId { get; set; }
        public virtual Business Business { get; set; }
    }
}
