using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model.Data
{
    /// <summary>
    /// 商户表
    /// </summary>
    [Table("Business")]
    public class Business : BaseEntity, ICloneable
    {
        /// <summary>
        /// 对象id，用于商户自动登录
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegisterDate { get; set; }
        /// <summary>
        /// 商户备注
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 营业执照
        /// </summary>
        public string BusinessLicense { get; set; }
        /// <summary>
        /// 营业执照地址
        /// </summary>
        public string BusinessLicenseImage { get; set; }
        /// <summary>
        /// 特殊资质
        /// </summary>
        public string SpecialImage { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 公众号id
        /// </summary>
        public string WeChatAppId { get; set; }
        /// <summary>
        /// 公众号密钥
        /// </summary>
        public string WeChatSecret { get; set; }
        /// <summary>
        /// 小程序id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 小程序App Secret
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 商户密钥
        /// </summary>
        public string MchKey { get; set; }
        /// <summary>
        /// 新订单通知模版消息id（公众号）
        /// </summary>
        public string NewOrderTemplateId { get; set; }
        /// <summary>
        /// 退款通知的模版消息id（公众号）
        /// </summary>
        public string RefundTemplateId { get; set; }
        /// <summary>
        /// 订单交易模版Id（小程序）
        /// </summary>
        public string TemplateNotifyId { get; set; }
        /// <summary>
        /// 支付服务商AppId
        /// </summary>
        public string PayServerAppId { get; set; }
        /// <summary>
        /// 支付服务商密钥
        /// </summary>
        public string PayServerKey { get; set; }
        /// <summary>
        /// 支付服务商商户号
        /// </summary>
        public string PayServerMchId { get; set; }
        /// <summary>
        /// 退款证书文件名（文件需放在目录Asserts下）
        /// </summary>
        public string CertFile { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// LOGO地址
        /// </summary>
        public string LogoSrc { get; set; }
        /// <summary>
        /// 是否自动接单
        /// </summary>
        public bool IsAutoReceipt { get; set; }
        /// <summary>
        /// 运费计算模式
        /// </summary>
        public FreightMode FreightMode { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public double? Freight { get; set; }
        /// <summary>
        /// 商户编号
        /// </summary>
        public string DadaSourceId { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string DadaShopNo { get; set; }
        /// <summary>
        /// 商户所属的城市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 商户所属城市的区号
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 配送范围
        /// </summary>
        public double Range { get; set; } = 0;
        /// <summary>
        /// 飞印商户编码
        /// </summary>
        public string FeyinMemberCode { get; set; }
        /// <summary>
        /// 飞印API密钥
        /// </summary>
        public string FeyinApiKey { get; set; }
        /// <summary>
        /// 默认打印设备
        /// </summary>
        public string DefaultPrinterDevice { get; set; }
        /// <summary>
        /// 商铺位置经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 商铺位置纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        [NotMapped]
        public double Distance { get; set; }
        /// <summary>
        /// 经营开始时间1
        /// </summary>
        public string BusinessStartTime { get; set; }
        /// <summary>
        /// 经营结束时间1
        /// </summary>
        public string BusinessEndTime { get; set; }
        /// <summary>
        /// 经营开始时间2
        /// </summary>
        public string BusinessStartTime2 { get; set; }
        /// <summary>
        /// 经营结束时间2
        /// </summary>
        public string BusinessEndTime2 { get; set; }
        /// <summary>
        /// 经营开始时间3
        /// </summary>
        public string BusinessStartTime3 { get; set; }
        /// <summary>
        /// 经营结束时间3
        /// </summary>
        public string BusinessEndTime3 { get; set; }
        /// <summary>
        /// 起送金额
        /// </summary>
        public double MinAmount { get; set; }
        /// <summary>
        /// 是否打烊
        /// </summary>
        public bool IsClose { get; set; }
        /// <summary>
        /// 配送服务商
        /// </summary>
        public LogisticsType ServiceProvider { get; set; }
        /// <summary>
        /// 是否正式发布
        /// </summary>
        public bool IsPublish { get; set; } = true;
        /// <summary>
        /// 绑定监听服务的二维码地址
        /// </summary>
        public string WxQrListenPath { get; set; }
        /// <summary>
        /// 小程序二维码字符串
        /// </summary>
        public string AppQrCode { get; set; }
        /// <summary>
        /// 商户类别
        /// </summary>
        public BusinessCategory Category { get; set; }
        /// <summary>
        /// 商家评分
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// 配送评分
        /// </summary>
        public double Delivery { get; set; }
        /// <summary>
        /// 一城飞客key
        /// </summary>
        public string YcfkKey { get; set; }
        /// <summary>
        /// 一城飞客secret
        /// </summary>
        public string YcfkSecret { get; set; }
        /// <summary>
        /// 订单可享受折扣商品数量
        /// </summary>
        public int DiscountQuantity { get; set; }
        /// <summary>
        /// 优惠活动是否同时享受
        /// </summary>
        public bool IsEnjoymentActivity { get; set; }
        /// <summary>
        /// 美团订单是否自动接单
        /// </summary>
        public bool MT_AutoRecieved { get; set; }
        /// <summary>
        /// 美团应用id
        /// </summary>
        public string MT_AppId { get; set; }
        /// <summary>
        /// 美团应用key
        /// </summary>
        public string MT_AppKey { get; set; }
        /// <summary>
        /// 美团门店id
        /// </summary>
        public string MT_Poi_Id { get; set; }
        /// <summary>
        /// 饿了么应用id
        /// </summary>
        public long? Eleme_AppId { get; set; }
        /// <summary>
        /// 饿了么应用KEY
        /// </summary>
        public string Eleme_AppKey { get; set; }
        /// <summary>
        /// 饿了么应用密钥
        /// </summary>
        public string Eleme_AppSecret { get; set; }
        /// <summary>
        /// 饿了么门店id
        /// </summary>
        public long? Eleme_Poi_Id { get; set; }
        /// <summary>
        /// 顺丰同城开发者id
        /// </summary>
        public string ShunfengDevId { get; set; }
        /// <summary>
        /// 顺丰同城开发者密钥
        /// </summary>
        public string ShunfengDevKey { get; set; }
        /// <summary>
        /// 顺丰门店id
        /// </summary>
        public string ShunfengShopId { get; set; }

        /// <summary>
        /// 所属连锁店的id
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 所属连锁店实例
        /// </summary>
        public virtual Business Parent { get; set; }
        /// <summary>
        /// 点我达商户对象
        /// </summary>
        //public int? DWD_BusinessId { get; set; }
        /// <summary>
        /// 点我达商户对象
        /// </summary>
        public virtual DWDStore DWDStore { get; set; }
        /// <summary>
        /// 产品列表集合
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
        /// <summary>
        /// 产品类别集合
        /// </summary>
        public virtual ICollection<ProductType> ProductsTypes { get; set; }
        /// <summary>
        /// 商户订单集合
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
        /// <summary>
        /// 飞印打印机集合
        /// </summary>
        public virtual ICollection<FeyinDevice> FeyinDevices { get; set; }
        /// <summary>
        /// 购物车集合
        /// </summary>
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        /// <summary>
        /// 满减活动
        /// </summary>
        public virtual ICollection<SaleFullReduce> SaleFullReduces { get; set; }
        /// <summary>
        /// 商品折扣活动
        /// </summary>
        public virtual ICollection<SaleProductDiscount> SaleProductDiscounts { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
