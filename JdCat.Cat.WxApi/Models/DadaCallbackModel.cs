using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.WxApi.Models
{
    public class DadaCallbackModel
    {
        public string client_id { get; set; }
        public string order_id { get; set; }
        public int order_status { get; set; }

        public string cancel_reason { get; set; }
        public int cancel_from { get; set; }
        public int update_time { get; set; }
        public string signature { get; set; }
        public int dm_id { get; set; }
        public string dm_name { get; set; }
        public string dm_mobile { get; set; }

    }
}
