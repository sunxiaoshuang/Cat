using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_UserList
    {
        [Description("头像")]
        public string Face { get; set; }
        [Description("昵称")]
        public string Nickname { get; set; }
        [Description("性别")]
        public UserGender Gender { get; set; }
        [Description("地域")]
        public string Area { get; set; }
        [Description("手机号")]
        public string Phone { get; set; }
        [Description("消费次数")]
        public int Quantity { get; set; }
        [Description("注册时间")]
        public DateTime? RegisteTime { get; set; }
        [Description("注册门店")]
        public string StoreName { get; set; }
    }
}
