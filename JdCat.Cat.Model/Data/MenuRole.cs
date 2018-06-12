using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 菜单角色对应关系表，用来配置多对多关系
    /// </summary>
    public class MenuRole : BaseEntity
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
