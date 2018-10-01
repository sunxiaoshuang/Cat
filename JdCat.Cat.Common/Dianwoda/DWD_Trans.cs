using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    /// <summary>
    /// 点我达传输对象
    /// </summary>
    public class DWD_Trans : DWDProperty<DWD_Trans>
    {
        public static string secret = "80a5cadfc25f8bafb10d644eb89aa52e";
        public string pk { get; set; } = "10217";
        public string sig { get; set; }
        public long timestamp { get; set; }
        public string format { get; set; } = "json";
        //public string @params { get; set; }
    }
}
