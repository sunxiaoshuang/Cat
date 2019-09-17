using JdCat.Cat.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 订单活动类
    /// </summary>
    [Table("TangOrderActivity")]
    public class TangOrderActivity : BaseEntityClient
    {
        /// <summary>
        /// 活动金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 活动类别
        /// </summary>
        public OrderActivityType Type { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public int? ActivityId { get; set; }
        /// <summary>
        /// 活动备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string TangOrderObjectId { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int TangOrderId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        public virtual TangOrder TangOrder { get; set; }
    }
}
