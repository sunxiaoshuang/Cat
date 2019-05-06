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
    [Table("Product")]
    public class Product : BaseEntity, ICloneable
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 名称拼音
        /// </summary>
        public string Pinyin { get; set; }
        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string FirstLetter { get; set; }
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
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
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
        public double? MinBuyQuantity { get; set; } = 1;
        /// <summary>
        /// 产品状态
        /// </summary>
        public ProductStatus Status { get; set; } = ProductStatus.Sale;
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime? NotSaleTime { get; set; }
        /// <summary>
        /// 备用属性
        /// </summary>
        [NotMapped]
        public object Tag1 { get; set; }
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
        /// 订单商品集合
        /// </summary>
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
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
        /// <summary>
        /// 商品折扣活动集合
        /// </summary>
        public virtual ICollection<SaleProductDiscount> SaleProductDiscount { get; set; }
        public object Clone()
        {
            var self = (Product)MemberwiseClone();
            if(self.Attributes != null)
            {
                self.Attributes = self.Attributes.Select(a => (ProductAttribute)a.Clone()).ToList();
            }
            if (self.Formats != null)
            {
                self.Formats = self.Formats.Select(a => (ProductFormat)a.Clone()).ToList();
            }
            if (self.Images != null)
            {
                self.Images = self.Images.Select(a => (ProductImage)a.Clone()).ToList();
            }
            return self;
        }
    }
}
