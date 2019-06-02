using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 堂食订单支付方式
    /// </summary>
    [Table("TangOrderPayment")]
    public class TangOrderPayment: BaseEntityClient
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 支付方式id（远程）
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// 支付方式id（本地）
        /// </summary>
        public string PaymentTypeObjectId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentType PaymentType { get; set; }
        /// <summary>
        /// 所属堂食订单id（本地）
        /// </summary>
        public string OrderObjectId { get; set; }
        /// <summary>
        /// 所属堂食订单id（远程）
        /// </summary>
        public int TangOrderId { get; set; }
        /// <summary>
        /// 所属堂食订单
        /// </summary>
        public TangOrder TangOrder { get; set; }
    }
}
