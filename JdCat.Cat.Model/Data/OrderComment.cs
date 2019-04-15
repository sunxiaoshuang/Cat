using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 订单评论表
    /// </summary>
    [Table("OrderComment")]
    public class OrderComment : BaseEntity
    {
        /// <summary>
        /// 评论用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 评论用户头像地址
        /// </summary>
        public string Face { get; set; }
        /// <summary>
        /// 评论用户手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 送达时间
        /// </summary>
        public DateTime ArrivedTime { get; set; }
        /// <summary>
        /// 实际送达时间（用户修改过的时间）
        /// </summary>
        public DateTime? ActualTime { get; set; }
        /// <summary>
        /// 配送得分
        /// </summary>
        public CommentLevel DeliveryScore { get; set; }
        /// <summary>
        /// 配送类型
        /// </summary>
        public LogisticsType DeliveryType { get; set; }
        /// <summary>
        /// 配送结果
        /// </summary>
        public string DeliveryResult { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentContent { get; set; }
        /// <summary>
        /// 评论结果
        /// </summary>
        public string CommentResult { get; set; }
        /// <summary>
        /// 评论得分
        /// </summary>
        public CommentLevel OrderScore { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 评论所属商户id
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
        /// <summary>
        /// 评论的订单id
        /// </summary>
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// 评论的用户id
        /// </summary>
        public int UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// 关联评论的菜品
        /// </summary>
        [NotMapped]
        public List<OrderProduct> OrderProducts { get; set; }

    }
}
