using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 小程序授权信息
    /// </summary>
    [Table("OpenAuthInfo", Schema = "dbo")]
    public class OpenAuthInfo : BaseEntity
    {
        /// <summary>
        /// 小程序id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 刷新Token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// 权限说明
        /// </summary>
        public string AuthNote { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 绑定的商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 绑定商户
        /// </summary>
        public virtual Business Business { get; set; }
    }
}
