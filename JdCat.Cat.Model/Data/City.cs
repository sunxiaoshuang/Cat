using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 城市名称与编码对应表
    /// </summary>
    [Table("City")]
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
