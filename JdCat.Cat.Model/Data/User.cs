using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.Model.Data
{
    [Table("User", Schema = "dbo")]
    public class User : BaseEntity
    {
        public int Age { get; set; } = 0;
        public string AvatarUrl { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public UserGender Gender { get; set; }
        public string Language { get; set; }
        public string NickName { get; set; }
        public string Province { get; set; }
        public string Phone { get; set; }
        /// <summary>
        /// 微信唯一标识码
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 是否已经注册
        /// </summary>
        public bool IsRegister { get; set; }
        /// <summary>
        /// 是否获取了手机号
        /// </summary>
        public bool IsPhone { get; set; }
        /// <summary>
        /// 依赖注册的商户
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 微信的会话密钥
        /// </summary>
        [NotMapped]
        public int Skey { get; set; }
    }
}
