using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    public class DWD_Cost : DWDProperty<DWD_Cost>
    {
        /// <summary>
        /// 是否是预约单（1:是，0:否）
        /// </summary>
        public int order_is_reserve { get; set; }
        /// <summary>
        /// 订单商品类型（默认可传00）
        /// </summary>
        public string cargo_type { get; set; } = "00";
        /// <summary>
        /// 订单商品重量，单位：克；（若无则传0）
        /// </summary>
        public int cargo_weight { get; set; } = 0;
        /// <summary>
        /// 行政区划代码，如杭州：330100
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        public string seller_id { get; set; }
        /// <summary>
        /// 商家店铺名称
        /// </summary>
        public string seller_name { get; set; }
        /// <summary>
        /// 商家联系方式
        /// </summary>
        public string seller_mobile { get; set; }
        /// <summary>
        /// 商家文字地址
        /// </summary>
        public string seller_address { get; set; }
        /// <summary>
        /// 商家纬度(高德火星坐标系)
        /// </summary>
        public double seller_lat { get; set; }
        /// <summary>
        /// 商家经度(高德火星坐标系)
        /// </summary>
        public double seller_lng { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string consignee_address { get; set; }
        /// <summary>
        /// 收货人纬度(高德火星坐标系)
        /// </summary>
        public double consignee_lat { get; set; }
        /// <summary>
        /// 收货人经度(高德火星坐标系)
        /// </summary>
        public double consignee_lng { get; set; }

    }
}
