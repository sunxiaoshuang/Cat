using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 微信会员
    /// </summary>
    [Table("WxMember")]
    public class WxMember : BaseEntity
    {
        public string Code { get; set; }
        /// <summary>
        /// 会员姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 会员昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public UserGender Gender { get; set; }
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public double Bonus { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public double RechargeAmount { get; set; }
        /// <summary>
        /// 消费次数
        /// </summary>
        public int PurchaseTimes { get; set; }
        /// <summary>
        /// 会员卡id
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// 简单猫微信会员卡id
        /// </summary>
        public int WxCardId { get; set; }
        public virtual WxCard WxCard { get; set; }
        /// <summary>
        /// 所属门店id
        /// </summary>
        public int BusinessId { get; set; }
        public virtual Business Business { get; set; }
    }
}
