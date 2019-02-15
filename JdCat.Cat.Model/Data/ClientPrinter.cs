using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 城市名称与编码对应表
    /// </summary>
    [Table("ClientPrinter")]
    public class ClientPrinter : BaseEntity
    {
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 打印机ip地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 打印机端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 打印机类型，[1:前台，2：后厨]
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 打印机当前状态，[1:正常，2:停用]
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 每次打印数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 0：一菜一打，1：一份一打，2：一单一打
        /// </summary>
        public int Mode { get; set; }
        /// <summary>
        /// 打印机规格：58mm或80mm
        /// </summary>
        public int Format { get; set; }
        /// <summary>
        /// 打印机关联的菜品id
        /// </summary>
        public string FoodIds { get; set; }
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
    }
}
