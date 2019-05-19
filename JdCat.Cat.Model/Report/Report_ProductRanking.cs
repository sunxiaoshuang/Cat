using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_ProductRanking
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 下单数量
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// 下单总额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 取消数量
        /// </summary>
        public double CancelQuantity { get; set; }
        /// <summary>
        /// 取消商品总额
        /// </summary>
        public double CancelSaleAmount { get; set; }
        /// <summary>
        /// 取消商品净额
        /// </summary>
        public double CancelAmount { get; set; }
        /// <summary>
        /// 销售数量
        /// </summary>
        public double SaleQuantity { get; set; }
        /// <summary>
        /// 销售总额
        /// </summary>
        public double SaleAmount { get; set; }
        /// <summary>
        /// 招待数量
        /// </summary>
        public double EntertainQuantity { get; set; }
        /// <summary>
        /// 招待总额
        /// </summary>
        public double EntertainAmount { get; set; }
        /// <summary>
        /// 折扣总额
        /// </summary>
        public double DiscountAmount { get; set; }
        /// <summary>
        /// 折扣商品数量
        /// </summary>
        public double DiscountQuantity { get; set; }
        /// <summary>
        /// 折后总额
        /// </summary>
        public double DiscountedAmount { get; set; }
        /// <summary>
        /// 净售数量
        /// </summary>
        public double ActualQuantity { get; set; }
        /// <summary>
        /// 净售总额
        /// </summary>
        public double ActualAmount { get; set; }
    }
}
