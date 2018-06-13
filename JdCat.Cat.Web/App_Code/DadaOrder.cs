using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Web.App_Code
{
    public class DadaOrder
    {
        public string shop_no { get; set; } = "73753";
        public string origin_id { get; set; } = "JD-291092";
        public string city_code { get; set; } = "027";
        public double cargo_price { get; set; }
        public int is_prepay { get; set; } = 0;
        public string receiver_name { get; set; }
        public string receiver_address { get; set; }
        public double receiver_lat { get; set; }
        public double receiver_lng { get; set; }
        public string callback { get; set; }
        public string receiver_phone { get; set; } = "17354300837";
        public string receiver_tel { get; set; }
        public double tips { get; set; }
        public string info { get; set; } = "备注";
        public int cargo_type { get; set; } = 1;
        public double cargo_weight { get; set; }
        public int cargo_num { get; set; }
        public string invoice_title { get; set; } = "简单猫科技（武汉）有限公司";
        public string origin_mark { get; set; } = "JdCat";
        public string origin_mark_no { get; set; } = "JdCat001";
        public int is_use_insurance { get; set; } = 0;
        public int is_finish_code_needed { get; set; } = 0;
        public int delay_publish_time { get; set; } = 0;
    }
}
