using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 餐桌分类
    /// </summary>
    [Table("DeskType")]
    public class DeskType : BaseEntity
    {
        public string Name { get; set; }
        public int Sort { get; set; }
        public bool IsDelete { get; set; }
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }
    }
}
