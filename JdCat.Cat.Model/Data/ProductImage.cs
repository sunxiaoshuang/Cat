using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("ProductImage", Schema = "dbo")]
    public class ProductImage : BaseEntity
    {
        public long Length { get; set; }
        public string ExtensionName{ get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
