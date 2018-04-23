using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("Business", Schema = "dbo")]
    public class Business : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string Log { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
