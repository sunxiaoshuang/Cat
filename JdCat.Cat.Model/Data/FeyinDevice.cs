﻿using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 飞印绑定的打印机记录
    /// </summary>
    [Table("FeyinDevice", Schema = "dbo")]
    public class FeyinDevice : BaseEntity
    {
        /// <summary>
        /// 商户编码
        /// </summary>
        public string MemberCode { get; set; }
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// 打印机编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string Name { get; set; }
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public virtual Business Business { get; set; }
    }
}