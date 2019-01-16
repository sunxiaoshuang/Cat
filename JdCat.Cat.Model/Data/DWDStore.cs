using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 点我达商户表
    /// </summary>
    [Table("DWDStore")]
    public class DWDStore : BaseEntity
    {
        /// <summary>
        /// 行政区划代码
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 行政区划名称
        /// </summary>
        public string city_name { get; set; }
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
        public double lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 点我达充值记录
        /// </summary>
        public virtual ICollection<DWD_Recharge> DWD_Recharges { get; set; }
    }
}
