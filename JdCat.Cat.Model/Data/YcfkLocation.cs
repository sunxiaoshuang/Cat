using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    [Table("YcfkLocation")]
    public class YcfkLocation : BaseEntity
    {
        public int OrderId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
