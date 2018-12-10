using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Repository.Model
{
    public class WmgjReturn<T>
    {
        public string msg { get; set; }
        public int errno { get; set; }
        public string msg_id { get; set; }
        public T data { get; set; }
    }
}
