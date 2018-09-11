using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 点我达新增订单信息
    /// </summary>
    public class DWD_Order : DWDProperty<DWD_Order>
    {
        /// <summary>
        /// 渠道订单编号
        /// </summary>
        public string order_original_id { get; set; }
        /// <summary>
        /// 渠道订单创建时间，时间戳
        /// </summary>
        public long order_create_time { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string order_remark { get; set; }
        /// <summary>
        /// 订单金额，单位：分
        /// </summary>
        public int order_price { get; set; }
        /// <summary>
        /// 是否是预约单，[1:是,0:不是]，非必填
        /// </summary>
        public int order_is_reserve { get; set; }
        /// <summary>
        /// 订单流水号，非必填
        /// </summary>
        public string serial_id { get; set; }
        /// <summary>
        /// 订单商品类型，参考类型编码表，非必填
        /// </summary>
        public string cargo_type { get; set; } = "00";
        /// <summary>
        /// 订单商品重量，单位：克
        /// </summary>
        public int cargo_weight { get; set; }
        /// <summary>
        /// 商品份数
        /// </summary>
        public int cargo_num { get; set; }
        /// <summary>
        /// 行政区划代码
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        public string seller_id { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string seller_name { get; set; }
        /// <summary>
        /// 商家联系方式
        /// </summary>
        public string seller_mobile { get; set; }
        /// <summary>
        /// 商家地址
        /// </summary>
        public string seller_address { get; set; }
        /// <summary>
        /// 商家纬度
        /// </summary>
        public double seller_lat { get; set; }
        /// <summary>
        /// 商家经度
        /// </summary>
        public double seller_lng { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string consignee_name { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>
        public string consignee_mobile { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string consignee_address { get; set; }
        /// <summary>
        /// 收货人纬度
        /// </summary>
        public double consignee_lat { get; set; }
        /// <summary>
        /// 收货人经度
        /// </summary>
        public double consignee_lng { get; set; }
        /// <summary>
        /// 配送员到店是否需要垫付，[1:是,0:不是]
        /// </summary>
        public int money_rider_needpaid { get; set; } = 0;
        /// <summary>
        /// 配送员到店需要垫付的金额
        /// </summary>
        public int money_rider_prepaid { get; set; }
        /// <summary>
        /// 配送员送达客户时，向客户收取的费用
        /// </summary>
        public int money_rider_charge { get; set; }
        /// <summary>
        /// 配送员应到店时间，时间戳，非必填
        /// </summary>
        public long? time_ready_for_deliver { get; set; }
        /// <summary>
        /// 在商家处等待所需的事件
        /// </summary>
        public int time_waiting_at_seller { get; set; }
        /// <summary>
        /// 用户选择的期望送达时间，非必填
        /// </summary>
        public long? time_expected_arrival { get; set; }
        /// <summary>
        /// 渠道支付配送费，单位：分
        /// </summary>
        public int delivery_fee_from_seller { get; set; }
        /// <summary>
        /// 商品信息，json
        /// </summary>
        public string items { get; set; }

    }
}
