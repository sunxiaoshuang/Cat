using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("ThirdProductMapping")]
    public class ThirdProductMapping: BaseEntity
    {
        /// <summary>
        /// 本地商品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 第三方商品id
        /// </summary>
        public string ThirdProductId { get; set; }
        /// <summary>
        /// 第三方商品名称
        /// </summary>
        public string ThirdProductName { get; set; }
        /// <summary>
        /// 第三方来源（0:美团，1:饿了么）
        /// </summary>
        public int ThirdSource { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 商户实体
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
