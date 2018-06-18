using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 达达取消订单原因
    /// </summary>
    [Table("DadaCancelReason", Schema = "dbo")]
    public class DadaCancelReason : BaseEntity
    {
        /// <summary>
        /// 取消原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public int FlagId { get; set; }

    }
}
