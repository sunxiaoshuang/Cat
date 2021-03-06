﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 产品属性配置表
    /// </summary>
    [Table("SettingProductAttribute")]
    public class SettingProductAttribute : BaseEntity
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性排序码
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 属性级别
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 父级属性id
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 子级属性集合
        /// </summary>
        public virtual ICollection<SettingProductAttribute> Childs { get; set; }
        /// <summary>
        /// 父级属性实体
        /// </summary>
        public virtual SettingProductAttribute Parent { get; set; }
    }
}
