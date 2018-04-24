using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    public class MenuRole : BaseEntity
    {
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
