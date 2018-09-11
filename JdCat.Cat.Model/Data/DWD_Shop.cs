using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 点我达订单回调信息对象
    /// </summary>
    public class DWD_Shop
    {
        /// <summary>
        /// 行政区划代码
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 外部商家编号
        /// </summary>
        public string external_shopid { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string shop_title { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public long lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public long lat { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string addr { get; set; }
    }
}
