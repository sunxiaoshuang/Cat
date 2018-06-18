using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 达达回调时，传递的对象
    /// </summary>
    [Table("DadaCallBack", Schema = "dbo")]
    public class DadaCallBack : BaseEntity
    {
        /// <summary>
        /// 达达运单号
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 系统订单号
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public DadaStatus order_status { get; set; }
        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string cancel_reason { get; set; }
        /// <summary>
        /// 订单取消原因来源
        /// </summary>
        public DadaCancelSource cancel_from { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int update_time { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 达达配送员id
        /// </summary>
        public int dm_id { get; set; }
        /// <summary>
        /// 配送员姓名
        /// </summary>
        public string dm_name { get; set; }
        /// <summary>
        /// 配送员手机号
        /// </summary>
        public string dm_mobile { get; set; }
        public int OrderId { get; set; }
        /// <summary>
        /// 对应的系统订单
        /// </summary>
        public virtual Order Order { get; set; }
    }
}
