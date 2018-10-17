﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common
{
    public class AppData
    {
        private static JsonSerializerSettings jsonSetting = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public static JsonSerializerSettings JsonSetting { get => jsonSetting; }

        /// <summary>
        /// API请求的地址
        /// </summary>
        public string ApiUri { get; set; }
        /// <summary>
        /// 文件服务器
        /// </summary>
        public string FileUri { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户信息存储Session名称
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        /// 存储用户Cookie的名称
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 新订单通知接口
        /// </summary>
        public string OrderUrl { get; set; }
        /// <summary>
        /// 运行环境
        /// </summary>
        public string RunMode { get; set; }
        /// <summary>
        /// 达达接口域名
        /// </summary>
        public string DadaDomain { get; set; }
        /// <summary>
        /// 简单猫开发者标识
        /// </summary>
        public string DadaAppKey { get; set; }
        /// <summary>
        /// 简单猫开发者密码
        /// </summary>
        public string DadaAppSecret { get; set; }
        /// <summary>
        /// 绑定到开发者账号中的某个商户Id
        /// </summary>
        public string DadaSourceId { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string DadaShopNo { get; set; }
        /// <summary>
        /// 回调URL
        /// </summary>
        public string DadaCallback { get; set; }
        /// <summary>
        /// 飞印应用id
        /// </summary>
        public string FeyinAppId { get; set; }
        /// <summary>
        /// 飞印商户编码
        /// </summary>
        public string FeyinMemberCode { get; set; }
        /// <summary>
        /// 飞印API密钥
        /// </summary>
        public string FeyinApiKey { get; set; }
        /// <summary>
        /// 支付服务商AppId
        /// </summary>
        public string ServerAppId { get; set; }
        /// <summary>
        /// 支付服务商Key
        /// </summary>
        public string ServerKey { get; set; }
        /// <summary>
        /// 支付服务商号
        /// </summary>
        public string ServerMchId { get; set; }
        /// <summary>
        /// 点我达接口地址
        /// </summary>
        public string DwdDomain { get; set; }
        /// <summary>
        /// 点我达开发者id
        /// </summary>
        public string DwdAppKey { get; set; }
        /// <summary>
        /// 点我达开发者密钥
        /// </summary>
        public string DwdAppSecret { get; set; }
        /// <summary>
        /// 点我达测试商户编码
        /// </summary>
        public string DwdShopNo { get; set; }
        /// <summary>
        /// 点我达接口回调
        /// </summary>
        public string DwdCallback { get; set; }
        /// <summary>
        /// 微信支付成功后通知地址
        /// </summary>
        public string PaySuccessUrl { get; set; }
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string HostIpAddress { get; set; }
        /// <summary>
        /// 公众号发送订单消息模版id
        /// </summary>
        public string EventMessageTemplateId { get; set; }
    }
}
