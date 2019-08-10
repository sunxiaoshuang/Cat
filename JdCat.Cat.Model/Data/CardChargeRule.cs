using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 会员卡充值规则
    /// </summary>
    [Table("CardChargeRule")]
    public class CardChargeRule : BaseEntity
    {
        public double Amount { get; set; }
        public double Give { get; set; }

        public int WxCardId { get; set; }
        public virtual WxCard WxCard { get; set; }

    }
}
