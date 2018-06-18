using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    public class DadaOrder
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string shop_no { get; set; } = "73753";
        /// <summary>
        /// 简单猫订单号
        /// </summary>
        public string origin_id { get; set; } = "JD-291092";
        /// <summary>
        /// 订单所在城市区号
        /// </summary>
        public string city_code { get; set; } = "027";
        /// <summary>
        /// 订单金额
        /// </summary>
        public double cargo_price { get; set; }
        /// <summary>
        /// 是否需要垫付 1:是 0:否 (垫付订单金额，非运费)
        /// </summary>
        public int is_prepay { get; set; } = 0;
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string receiver_name { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string receiver_address { get; set; }
        /// <summary>
        /// 收货人地址维度（高德坐标系）
        /// </summary>
        public double receiver_lat { get; set; }
        /// <summary>
        /// 收货人地址经度（高德坐标系）
        /// </summary>
        public double receiver_lng { get; set; }
        /// <summary>
        /// 回调URL
        /// </summary>
        public string callback { get; set; }
        /// <summary>
        /// 收货人手机号，以下属性均为非必填
        /// </summary>
        public string receiver_phone { get; set; } = "17354300837";
        /// <summary>
        /// 收货人座机号
        /// </summary>
        public string receiver_tel { get; set; }
        /// <summary>
        /// 小费
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string info { get; set; } = "备注";
        /// <summary>
        /// 订单商品类型：食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5
        /// </summary>
        public int cargo_type { get; set; } = 1;
        /// <summary>
        /// 订单重量（单位：Kg）
        /// </summary>
        public double cargo_weight { get; set; }
        /// <summary>
        /// 订单商品数量
        /// </summary>
        public int cargo_num { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string invoice_title { get; set; } = "简单猫科技（武汉）有限公司";
        /// <summary>
        /// 订单来源标示（该字段可以显示在达达app订单详情页面，只支持字母，最大长度为10）
        /// </summary>
        public string origin_mark { get; set; } = "JdCat";
        /// <summary>
        /// 订单来源编号（该字段可以显示在达达app订单详情页面，支持字母和数字，最大长度为30）
        /// </summary>
        public string origin_mark_no { get; set; } = "JdCat001";
        /// <summary>
        /// 是否使用保价费
        /// </summary>
        public int is_use_insurance { get; set; } = 0;
        /// <summary>
        /// 收货码
        /// </summary>
        public int is_finish_code_needed { get; set; } = 0;
        /// <summary>
        /// 预约发单时间
        /// </summary>
        public int delay_publish_time { get; set; } = 0;
        /// <summary>
        /// 是否选择直拿直送
        /// </summary>
        public int is_direct_delivery { get; set; }
    }
}
