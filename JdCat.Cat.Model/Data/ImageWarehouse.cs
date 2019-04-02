using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 图片仓库
    /// </summary>
    [Table("ImageWarehouse")]
    public class ImageWarehouse : BaseEntity
    {
        /// <summary>
        /// 关联表Id
        /// </summary>
        public int RelativeId { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public ImageType ImageType { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Src { get; set; }
    }
}
