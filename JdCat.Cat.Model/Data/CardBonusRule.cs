using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 会员卡积分规则
    /// </summary>
    [Table("CardBonusRule")]
    public class CardBonusRule : BaseEntity
    {
        public double Amount { get; set; }
        public double Give { get; set; }
        public BonusMode Mode { get; set; }

        public int WxCardId { get; set; }
        public virtual WxCard WxCard { get; set; }

    }
}
