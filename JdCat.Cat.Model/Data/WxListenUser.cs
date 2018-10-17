using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 城市名称与编码对应表
    /// </summary>
    [Table("WxListenUser", Schema = "dbo")]
    public class WxListenUser : BaseEntity
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }

    }
}
