using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    public class DWD_Recharge : BaseEntity
    {
        /// <summary>
        /// 充值编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 充值方式
        /// </summary>
        public DwdRechargeType Mode { get; set; }
        /// <summary>
        /// 是否充值完成
        /// </summary>
        public bool IsFinish { get; set; }
        /// <summary>
        /// 点我达充值流水
        /// </summary>
        public string DwdCode { get; set; }
        public int DWD_BusinessId { get; set; }
        public DWDStore DWD_Business { get; set; }

    }
}
