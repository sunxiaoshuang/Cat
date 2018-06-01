using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.WxApi.Models
{
    public class WxPhone
    {
        public string PhoneNumber { get; set; }
        public string PurePhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public WxPhoneMark Watermark { get; set; }
    }

    public class WxPhoneMark
    {
        public int Timestamp { get; set; }
        public string Appid { get; set; }
    }
}
