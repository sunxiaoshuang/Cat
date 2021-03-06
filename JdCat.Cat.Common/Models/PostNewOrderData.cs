﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.Common.Models
{
    public class PostNewOrderData
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// 订单内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceviceTime { get; set; }
    }
}
