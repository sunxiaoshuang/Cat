using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    public class DWD_Logistics
    {
        public DWD_OrderStatus order_status { get; set; }
        public long time_status_update { get; set; }
    }
}
