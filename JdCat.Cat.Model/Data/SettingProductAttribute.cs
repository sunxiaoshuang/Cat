using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("SettingProductAttribute", Schema = "dbo")]
    public class SettingProductAttribute : BaseEntity
    {
        public string Name { get; set; }
        public int Sort { get; set; }
        public int Level { get; set; }
        public int? ParentId { get; set; }
        public virtual ICollection<SettingProductAttribute> Childs { get; set; }
        public virtual SettingProductAttribute Parent { get; set; }
    }
}
