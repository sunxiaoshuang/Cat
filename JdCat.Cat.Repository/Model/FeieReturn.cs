using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Repository.Model
{
    public class FeieReturn
    {
        public string msg { get; set; }
        public int ret { get; set; }
        public object data { get; set; }
        public int serverExecutedTime { get; set; }
    }
}
