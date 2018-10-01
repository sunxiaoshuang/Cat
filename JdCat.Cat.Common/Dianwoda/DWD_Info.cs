using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    public class DWD_Info
    {
        /// <summary>
        /// 点我达订单编号
        /// </summary>
        public string dwd_order_id { get; set; }
        /// <summary>
        /// 渠道订单编号
        /// </summary>
        public string order_original_id { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public DWD_OrderStatus order_status { get; set; }
        /// <summary>
        /// 配送状态更新时间戳
        /// </summary>
        public long time_status_update { get; set; }
        /// <summary>
        /// 服务类型，[0:骑手,5:骑士]
        /// </summary>
        public int service_type { get; set; }
        /// <summary>
        /// 异常订单原因
        /// </summary>
        public string abnormal_reason { get; set; }
        /// <summary>
        /// 配送员编号
        /// </summary>
        public string rider_code { get; set; }
        /// <summary>
        /// 配送员姓名
        /// </summary>
        public string rider_name { get; set; }
        /// <summary>
        /// 配送员纬度
        /// </summary>
        public double rider_lat { get; set; }
        /// <summary>
        /// 配送员经度
        /// </summary>
        public double rider_lng { get; set; }
        /// <summary>
        /// 配送员手机号
        /// </summary>
        public string rider_mobile { get; set; }
        /// <summary>
        /// 配送员位置更新时间戳
        /// </summary>
        public long time_position_update { get; set; }
        /// <summary>
        /// 预估配送员到达商家时间戳
        /// </summary>
        public long time_estimated_pickup { get; set; }
        /// <summary>
        /// 预估配送员到达用户时间戳
        /// </summary>
        public long time_estimated_delivered { get; set; }
        /// <summary>
        /// 区域经理的姓名
        /// </summary>
        public string am_name { get; set; }
        /// <summary>
        /// 区域经理手机号码
        /// </summary>
        public string am_mobile { get; set; }
        /// <summary>
        /// 物流扩展信息
        /// </summary>
        public List<DWD_Logistics> logistic_info { get; set; }
    }
}
