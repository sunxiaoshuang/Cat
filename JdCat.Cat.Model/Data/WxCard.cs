using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 微信卡券表
    /// </summary>
    [Table("WxCard")]
    public class WxCard : BaseEntity
    {
        /// <summary>
        /// 卡券微信端id
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// 卡券名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 卡券类别
        /// </summary>
        public CardCategory Category { get; set; }
        /// <summary>
        /// 卡券颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 卡券状态
        /// </summary>
        public EntityStatus Status { get; set; }
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }

    }
}
