using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.WsService.Models
{
    /// <summary>
    /// 达达回调时，传递的对象
    /// </summary>
    public class DadaCallBackModel
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
    }

    public enum DadaStatus
    {
        /// <summary>
        /// 待接单
        /// </summary>
        PendingOrder = 1,
        /// <summary>
        /// 待取货
        /// </summary>
        DistributorReceipt = 2,
        /// <summary>
        /// 配送中
        /// </summary>
        Distribution = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 4,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 5,
        /// <summary>
        /// 已过期
        /// </summary>
        Expire = 7,
        /// <summary>
        /// 指派单
        /// </summary>
        AssignmentList = 8,
        /// <summary>
        /// 妥投异常之物品返回中
        /// </summary>
        Returning = 9,
        /// <summary>
        /// 妥投异常之物品返回完成
        /// </summary>
        Returned = 10,
        /// <summary>
        /// 创建达达运单失败
        /// </summary>
        Fail = 1000
    }
    public enum DadaCancelSource
    {
        /// <summary>
        /// 默认值
        /// </summary>
        Default = 0,
        /// <summary>
        /// 达达配送员取消
        /// </summary>
        Distributor = 1,
        /// <summary>
        /// 商家主动取消
        /// </summary>
        Business = 2,
        /// <summary>
        /// 系统或客服取消
        /// </summary>
        System = 3,
    }
}
