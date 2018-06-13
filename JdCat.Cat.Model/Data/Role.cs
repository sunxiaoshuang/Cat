using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Table("Role", Schema = "dbo")]
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
