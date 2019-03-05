using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 餐桌
    /// </summary>
    [Table("Desk")]
    public class Desk : BaseEntity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int BusinessId { get; set; }
        public bool IsDelete { get; set; }
        public virtual Business Business { get; set; }
        public int DeskTypeId { get; set; }
        public virtual DeskType DeskType { get; set; }
    }
}
