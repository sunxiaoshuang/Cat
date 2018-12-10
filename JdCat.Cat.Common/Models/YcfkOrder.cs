using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 订单对象（用于数据传递）
    /// </summary>
    public class YcfkOrder
    {
        /// <summary>
        /// 获取或者设置订单编号（第三方商家的订单编号）
        /// </summary>
        public string OrderId { get; set; } = "";
        /// <summary>
        /// 获取或者设置商家编号（第三方商家的商家编号）
        /// </summary>
        public string ShopId { get; set; } = "";
        /// <summary>
        /// 获取或者设置商家名称（第三方商家的商家名称）
        /// </summary>
        public string ShopName { get; set; } = "";
        /// <summary>
        /// 获取或者设置客户姓名
        /// </summary>
        public string OrderUserName { get; set; } = "";
        /// <summary>
        /// 获取或者设置客户电话
        /// </summary>
        public string OrderUserPhone { get; set; } = "";
        /// <summary>
        /// 获取或者设置客户地址
        /// </summary>
        public string OrderUserAddress { get; set; } = "";
        /// <summary>
        /// 获取或者设置客户备注
        /// </summary>
        public string OrderRemark { get; set; } = "";
        /// <summary>
        /// 获取或者设置支付方式【0：线下支付，1：线上支付】
        /// </summary>
        public int PayType { get; set; } = 1;
        /// <summary>
        /// 获取或者设置餐盒费
        /// </summary>
        public decimal BoxFee { get; set; }
        /// <summary>
        /// 获取或者设置运费
        /// </summary>
        public decimal Freight { get; set; }
        /// <summary>
        /// 获取或者设置活动减免金额（所有减免金额都算在此字段中）
        /// </summary>
        public decimal ActivityMoney { get; set; }
        /// <summary>
        /// 获取或者设置活动减免金额（商家承担部分）
        /// </summary>
        public decimal ShopActivityMoney { get; set; }
        /// <summary>
        /// 获取或者设置押金
        /// </summary>
        public decimal DepositMoney { get; set; }
        /// <summary>
        /// 获取或者设置预约到达时间（格式：yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string ReachTime { get; set; } = "";
        /// <summary>
        /// 获取或者设置发票类型【0为不要发票，1为个人发票，2为公司发票】
        /// </summary>
        public int Invoice { get; set; }
        /// <summary>
        /// 获取或者设置发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; } = "";
        /// <summary>
        /// 发票税号，当发票类型为2时必填
        /// </summary>
        public string TaxpayerId { get; set; } = "";
        /// <summary>
        /// 获取或者设置菜品集合对象
        /// </summary>
        public List<YcfkFoodItem> FoodList { get; set; }
        /// <summary>
        /// 获取或者设置订单标志
        /// </summary>
        public string Flag { get; set; } = "";
        /// <summary>
        /// 获取或者设置用户百度坐标
        /// </summary>
        public string UserBaiduCoordinate { get; set; } = "";
        /// <summary>
        /// 获取或者设置用户高德坐标
        /// </summary>
        public string UserGaodeCoordinate { get; set; } = "";
        /// <summary>
        /// 获取或者设置用户GPS坐标
        /// </summary>
        public string UserGPSCoordinate { get; set; } = "";
        /// <summary>
        /// 获取或者设置第三方商家的当日订单流水号
        /// </summary>
        public int DayIndex { get; set; }
        /// <summary>
        /// 获取或者设置展示订单编号
        /// </summary>
        public string ViewOrderId { get; set; } = "";
        /// <summary>
        /// 获取或者设置取餐类型（0为普通取餐，1为到店自取）
        /// </summary>
        public int PickType { get; set; }

    }
}
