using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Dianwoda
{
    public class DWD_Detail
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int page_count { get; set; }
        /// <summary>
        /// 交易明细
        /// </summary>
        public List<DWD_DetailList> detail { get; set; }
    }
}
