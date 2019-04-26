using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JdCat.Cat.Model.Report
{
    public class Report_Setmeal
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 销售数量
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// 关联的套餐
        /// </summary>
        public List<Tuple<string, double>> SetMeals { get; set; }

    }
}
