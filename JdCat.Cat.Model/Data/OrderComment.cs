using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 用户地址表
    /// </summary>
    [Table("OrderComment")]
    public class OrderComment : BaseEntity
    {
        public DateTime ArrivedTime { get; set; }
        public DateTime? ActualTime { get; set; }
        public CommentLevel DeliveryScore { get; set; }
        public LogisticsType DeliveryType { get; set; }
        public string DeliveryResult { get; set; }
        public DateTime CommentTime { get; set; }
        public string CommentContent { get; set; }
        public string CommentResult { get; set; }
        public CommentLevel OrderScore { get; set; }

        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
