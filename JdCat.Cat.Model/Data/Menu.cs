using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 菜单表
    /// </summary>
    [Table("Menu", Schema = "dbo")]
    public class Menu : BaseEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 视图名称
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 字体图标
        /// </summary>
        public string FontFace { get; set; }
        /// <summary>
        /// 父级菜单Id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 导航属性，父级菜单（顶级的为null）
        /// </summary>
        public virtual Menu Parent { get; set; }
        /// <summary>
        /// 所属子菜单
        /// </summary>
        public virtual ICollection<Menu> Menus { get; set; }
        /// <summary>
        /// 菜单所关联的角色
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
    }
}
