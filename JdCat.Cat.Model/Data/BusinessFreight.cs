using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    public class BusinessFreight : BaseEntity
    {
        /// <summary>
        /// 最大距离
        /// </summary>
        public double MaxDistance { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
    }
}
