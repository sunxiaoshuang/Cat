﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 商品属性表
    /// </summary>
    [Table("ProductAttribute")]
    public class ProductAttribute : BaseEntity, ICloneable
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性1
        /// </summary>
        public string Item1 { get; set; }
        /// <summary>
        /// 属性2
        /// </summary>
        public string Item2 { get; set; }
        /// <summary>
        /// 属性3
        /// </summary>
        public string Item3 { get; set; }
        /// <summary>
        /// 属性4
        /// </summary>
        public string Item4 { get; set; }
        /// <summary>
        /// 属性5
        /// </summary>
        public string Item5 { get; set; }
        /// <summary>
        /// 属性6
        /// </summary>
        public string Item6 { get; set; }
        /// <summary>
        /// 属性7
        /// </summary>
        public string Item7 { get; set; }
        /// <summary>
        /// 属性8
        /// </summary>
        public string Item8 { get; set; }
        /// <summary>
        /// 分类所属商家id
        /// </summary>
        [Required]
        public int ProductId { get; set; }
        /// <summary>
        /// 商品对象
        /// </summary>
        public virtual Product Product { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
