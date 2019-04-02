using JdCat.Cat.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 堂食订单商品
    /// </summary>
    [Table("TangOrderProduct")]
    public class TangOrderProduct: BaseEntityClient
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalPrice { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 套餐商品的id集
        /// </summary>
        public string ProductIdSet { get; set; }
        /// <summary>
        /// 商品特性
        /// </summary>
        public ProductFeature Feature { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        public TangOrderProductStatus ProductStatus { get; set; }
        /// <summary>
        /// 所属商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 所属商品实体
        /// </summary>
        public virtual Product Product { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        public virtual TangOrder TangOrder { get; set; }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string OrderObjectId { get; set; }
        /// <summary>
        /// 商品规格id
        /// </summary>
        public int FormatId { get; set; }
        /// <summary>
        /// 商品规格实体
        /// </summary>
        public virtual ProductFormat Format { get; set; }
    }
}
