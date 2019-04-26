using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 堂食订单
    /// </summary>
    [Table("TangOrder")]
    public class TangOrder : BaseEntityClient
    {
        /// <summary>
        /// 订单当日编号（为0表示还没有下单）
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public double OriginalAmount { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double PreferentialAmount { get; set; }
        /// <summary>
        /// 收到的金额
        /// </summary>
        public double ReceivedAmount { get; set; }
        /// <summary>
        /// 找赎
        /// </summary>
        public double GiveAmount { get; set; }
        /// <summary>
        /// 用餐人数
        /// </summary>
        public int PeopleNumber { get; set; }
        /// <summary>
        /// 餐位费
        /// </summary>
        public double MealFee { get; set; }
        /// <summary>
        /// 小费
        /// </summary>
        public double Tips { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 支付备注
        /// </summary>
        public string PaymentRemark { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public TangOrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public OrderSource OrderSource { get; set; }
        /// <summary>
        /// 订单类别
        /// </summary>
        public OrderMode OrderMode { get; set; }
        /// <summary>
        /// 支付方式id（远程）
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public virtual PaymentType PaymentType { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 所属员工id（远程）
        /// </summary>
        public int? StaffId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string StaffName { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        public virtual Staff Staff { get; set; }
        /// <summary>
        /// 对应的餐桌id
        /// </summary>
        public int? DeskId { get; set; }
        /// <summary>
        /// 对应的餐桌实体
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string DeskName { get; set; }
        /// <summary>
        /// 订单所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 商户实体
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 订单产品
        /// </summary>
        public virtual ICollection<TangOrderProduct> TangOrderProducts { get; set; }
    }
}
