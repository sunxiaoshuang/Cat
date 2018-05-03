﻿using JdCat.Cat.Model.Enum;
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
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商品标签，搜索的时候会用到，暂时不涉及
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        public virtual ICollection<ProductFormat> Formats { get; set; }
        /// <summary>
        /// 商品特色（后期可能使用这个属性，开发套餐的功能）
        /// </summary>
        public ProductFeature Feature { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        public virtual ICollection<ProductAttribute> Attributes { get; set; }
        /// <summary>
        /// 产品单位
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 最小购买量
        /// </summary>
        public decimal? MinBuyQuantity { get; set; } = 1;
        /// <summary>
        /// 产品状态
        /// </summary>
        public ProductStatus Status { get; set; } = ProductStatus.Init;
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime? NotSaleTime { get; set; }
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
        /// <summary>
        /// 产品修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}
