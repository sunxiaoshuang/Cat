using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 调用新增订单成功后，返回的点我达信息
    /// </summary>
    //[Table("DwdContent", Schema = "dbo")]
    public class DWD_Content
    {
        /// <summary>
        /// 点位达订单编号
        /// </summary>
        public string dwd_order_id { get; set; }
        /// <summary>
        /// 触发补贴的天气标签，仅即时单会有
        /// </summary>
        public string skycon { get; set; }
        /// <summary>
        /// 补贴金额，单位：分
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 配送距离，单位：米
        /// </summary>
        public int distance { get; set; }
        /// <summary>
        /// 渠道订单编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 对应的系统订单Id
        /// </summary>
        //public int OrderId { get; set; }
        //public virtual Order Order { get; set; }
    }
}
