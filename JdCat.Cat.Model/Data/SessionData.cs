using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("SessionData", Schema = "dbo")]
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
    }
}
