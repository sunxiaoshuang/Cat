using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    public class WxMenu
    {
        public string name { get; set; }
        public WxMenuCategory type { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public string appid { get; set; }
        public string pagepath { get; set; }
        public string media_id { get; set; }
        public List<WxMenu> sub_button { get; set; }
    }
    public enum WxMenuCategory
    {
        none = 0,
        click = 1,
        view = 2,
        miniprogram = 3,
        scancode_push = 4,
        scancode_waitmsg = 5,
        pic_sysphoto = 6,
        pic_photo_or_album = 7,
        pic_weixin = 8,
        location_select = 9,
        media_id = 10,
        view_limited = 11
    }
}
