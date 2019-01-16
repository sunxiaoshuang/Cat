using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 达达取消订单时，产生的违约金
    /// </summary>
    [Table("DadaLiquidatedDamages")]
    public class DadaLiquidatedDamages : BaseEntity
    {
        /// <summary>
        /// 违约金
        /// </summary>
        public double Deduct_fee { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
