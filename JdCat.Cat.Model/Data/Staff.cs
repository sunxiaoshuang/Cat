using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 商户员工表
    /// </summary>
    [Table("Staff")]
    public class Staff : BaseEntity
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 员工登录帐号
        /// </summary>
        public string Alise { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 员工编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public UserGender Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? EnterTime { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public EntityStatus Status { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 所属商户对象
        /// </summary>
        public virtual Business Business { get; set; }
        /// <summary>
        /// 员工岗位id
        /// </summary>
        public int StaffPostId { get; set; }
        /// <summary>
        /// 员工岗位对象
        /// </summary>
        public virtual StaffPost StaffPost { get; set; }
        /// <summary>
        /// 厨师菜品关系
        /// </summary>
        public virtual ICollection<CookProductRelative> CookProductRelatives { get; set; }
    }
}
