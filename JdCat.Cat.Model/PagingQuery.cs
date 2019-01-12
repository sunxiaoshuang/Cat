using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Model
{
    public class PagingQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public double PageCount
        {
            get
            {
                return Math.Ceiling(Convert.ToDouble(RecordCount) / Convert.ToDouble(PageSize));
            }
        }
        public int RecordCount { get; set; }

        public int Skip
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }
    }
}
