using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 用户订单表
    /// </summary>
    [Table("Order", Schema = "dbo")]
    public class Order : BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double? OldPrice { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public double? Freight { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 收货人手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 小费
        /// </summary>
        public double? Tips { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 餐具数量
        /// </summary>
        public int? TablewareQuantity { get; set; }
        /// <summary>
        /// 送货方式
        /// </summary>
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.Third;
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string WxPayCode { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.NotPay;
        /// <summary>
        /// 订单类别
        /// </summary>
        public OrderType Type { get; set; } = OrderType.Food;
        /// <summary>
        /// 用餐类别
        /// </summary>
        public OrderCategory Category { get; set; } = OrderCategory.TakeOut;
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentType PaymentType { get; set; } = PaymentType.OnLine;
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime? DistributionTime { get; set; }
        /// <summary>
        /// 送达时间
        /// </summary>
        public DateTime? AchieveTime { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RejectReasion { get; set; }
        /// <summary>
        /// 订单送达地址的城市编码
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 订单错误原因，记录配送失败原因以及其他各种原因
        /// </summary>
        public string ErrorReason { get; set; }
        /// <summary>
        /// 当日订单编号
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// 配送流水
        /// </summary>
        public int DistributionFlow { get; set; }
        /// <summary>
        /// 物流方式
        /// </summary>
        public LogisticsType LogisticsType { get; set; }
        /// <summary>
        /// 物流平台返回的费用
        /// </summary>
        public double? CallbackCost { get; set; }
        /// <summary>
        /// 微信调用统一支付接口后返回的id，用来发送模版消息，使用redis后删除掉
        /// </summary>
        public string PrepayId { get; set; }
        /// <summary>
        /// 下单人OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        /// <summary>
        /// 商户Id
        /// </summary>
        public int? BusinessId { get; set; }
        public virtual Business Business { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public virtual ICollection<OrderProduct> Products { get; set; }
        /// <summary>
        /// 达达订单回调结果集
        /// </summary>
        public virtual ICollection<DadaCallBack> DadaCallBacks { get; set; }
        /// <summary>
        /// 达达新增订单返回结果
        /// </summary>
        public virtual DadaReturn DadaReturn { get; set; }
        /// <summary>
        /// 是否已经发送过达达
        /// </summary>
        public bool IsSendDada { get; set; }
        /// <summary>
        /// 点我达新增订单返回结果
        /// </summary>
        //public virtual DWD_Content DwdContent{ get; set; }
        /// <summary>
        /// 违约金
        /// </summary>
        public virtual ICollection<DadaLiquidatedDamages> DadaLiquidatedDamages { get; set; }
        /// <summary>
        /// 满减活动id
        /// </summary>
        public int? SaleFullReduceId { get; set; }
        public virtual SaleFullReduce SaleFullReduce { get; set; }
        /// <summary>
        /// 满减优惠的金额
        /// </summary>
        public double? SaleFullReduceMoney { get; set; }
        /// <summary>
        /// 优惠券id
        /// </summary>
        public int? SaleCouponUserId { get; set; }
        public virtual SaleCouponUser SaleCouponUser { get; set; }
        /// <summary>
        /// 使用的优惠券金额
        /// </summary>
        public double? SaleCouponUserMoney { get; set; }
        /// <summary>
        /// 配送距离
        /// </summary>
        public int? Distance { get; set; }
    }
}
