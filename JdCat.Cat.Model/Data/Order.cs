﻿using JdCat.Cat.Model.Enum;
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
        public decimal? Price { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal? Freight { get; set; }
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
        public decimal? Tips { get; set; }
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
        /// 违约金
        /// </summary>
        public virtual ICollection<DadaLiquidatedDamages> DadaLiquidatedDamages { get; set; }
    }
}
