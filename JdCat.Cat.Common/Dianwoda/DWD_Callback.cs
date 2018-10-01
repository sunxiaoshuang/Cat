using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达订单回调信息对象
    /// </summary>
    public class DWD_Callback
    {
        /// <summary>
        /// 渠道订单编号
        /// </summary>
        public string order_original_id { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public DWD_OrderStatus order_status { get; set; }
        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string cancel_reason { get; set; }
        /// <summary>
        /// 订单异常原因
        /// </summary>
        public string abnormal_reason { get; set; }
        /// <summary>
        /// 妥投类型
        /// </summary>
        public string finish_reason { get; set; }
        /// <summary>
        /// 配送员编号
        /// </summary>
        public string rider_code { get; set; }
        /// <summary>
        /// 配送员姓名
        /// </summary>
        public string rider_name { get; set; }
        /// <summary>
        /// 配送员手机号
        /// </summary>
        public string rider_mobile { get; set; }
        /// <summary>
        /// 更新时间戳
        /// </summary>
        public long time_status_update { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sig { get; set; }
    }
}
