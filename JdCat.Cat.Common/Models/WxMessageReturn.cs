using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 微信模版消息返回值
    /// </summary>
    public class WxMessageReturn
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
