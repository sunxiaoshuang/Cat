﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("OrderProduct", Schema = "dbo")]
    public class OrderProduct : BaseEntity
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
        /// 商品总价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 描述（规格 + 属性）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}