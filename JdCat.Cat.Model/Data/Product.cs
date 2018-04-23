using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("Product", Schema = "dbo")]
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public DateTime PublishTime { get; set; }
        public string Description { get; set; }
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
    }
}
