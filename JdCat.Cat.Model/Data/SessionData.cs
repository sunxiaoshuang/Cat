using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 用户登录Session记录表
    /// </summary>
    [Table("SessionData")]
    public class SessionData : BaseEntity
    {
        /// <summary>
        /// 保存用户的SessionKey
        /// </summary>
        public string SessionKey { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户实例
        /// </summary>
        public virtual User User { get; set; }

    }
}
