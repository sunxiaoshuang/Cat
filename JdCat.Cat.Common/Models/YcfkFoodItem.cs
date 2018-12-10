using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 菜品对象
    /// </summary>
    public class YcfkFoodItem
    {
        /// <summary>
        /// 获取或者设置菜品名称
        /// </summary>
        public string FoodName { get; set; }
        /// <summary>
        /// 获取或者设置菜品单价
        /// </summary>
        public decimal FoodPrice { get; set; }
        /// <summary>
        /// 获取或者设置菜品数量
        /// </summary>
        public int FoodCount { get; set; }
    }
}
