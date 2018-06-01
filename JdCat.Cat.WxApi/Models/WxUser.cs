using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.WxApi.Models
{
    public class WxUser
    {
        public string OpenId { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public string Language { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string AvatarUrl { get; set; }
        public string UnionId { get; set; }
        public WaterMark WaterMark { get; set; }
    }

    public class WaterMark
    {
        public long Timestamp { get; set; }
        public string AppId { get; set; }
    }
}
