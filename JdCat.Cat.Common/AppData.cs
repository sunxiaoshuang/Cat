using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JdCat.Cat.Common
{
    public class AppData
    {
        public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static JsonSerializerSettings JsonSetting
        {
            get => new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public void Init(IConfiguration config)
        {
            var props = typeof(AppData).GetProperties();
            var appData = config.GetSection("appData");
            var items = appData.GetChildren();

            foreach (var prop in props)
            {
                var value = appData[prop.Name];
                if (string.IsNullOrEmpty(value)) continue;
                prop.SetValue(this, value);
            }
            Connection = config.GetConnectionString("CatContext");
        }

        /// <summary>
        /// API请求的地址
        /// </summary>
        public string ApiUri { get; set; }
        /// <summary>
        /// 文件服务器
        /// </summary>
        public string FileUri { get; set; }
        /// <summary>
        /// 网站域名
        /// </summary>
        public string Domain { get; set; }
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
        /// 易联云开发者id
        /// </summary>
        public string YlyPartnerId { get; set; }
        /// <summary>
        /// 易联云开发者密钥
        /// </summary>
        public string YlyApiKey { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string YlyUrl { get; set; }
        /// <summary>
        /// 飞鹅打印机开发者账号
        /// </summary>
        public string FeieUser { get; set; }
        /// <summary>
        /// 飞鹅打印机开发者密钥
        /// </summary>
        public string FeieKey { get; set; }
        /// <summary>
        /// 飞鹅接口地址
        /// </summary>
        public string FeieUrl { get; set; }
        /// <summary>
        /// 外卖管家打印信息接口地址
        /// </summary>
        public string WmgjUrl { get; set; }
        /// <summary>
        /// 外卖管家应用编号
        /// </summary>
        public string WmgjAppKey { get; set; }
        /// <summary>
        /// 外卖管家应用id
        /// </summary>
        public string WmgjAppId { get; set; }
        /// <summary>
        /// 开放平台AppId
        /// </summary>
        public string OpenAppId { get; set; }
        /// <summary>
        /// 开放平台Secret
        /// </summary>
        public string OpenSecret { get; set; }
        /// <summary>
        /// 开放平台加解密Key
        /// </summary>
        public string OpenEncodingAESKey { get; set; }
        /// <summary>
        /// 开放平台校验Token
        /// </summary>
        public string OpenToken { get; set; }
        /// <summary>
        /// 一城飞客key
        /// </summary>
        public string YcfkPartnerKey { get; set; }
        /// <summary>
        /// 一城飞客secret
        /// </summary>
        public string YcfkSecret { get; set; }
        /// <summary>
        /// 一城飞客域名
        /// </summary>
        public string YcfkDomain { get; set; }
        /// <summary>
        /// 公众号发送订单消息模版id
        /// </summary>
        public string EventMessageTemplateId { get; set; }
        /// <summary>
        /// 服务商退款证书
        /// </summary>
        public string CertFile { get; set; }
        /// <summary>
        /// 退款通知id
        /// </summary>
        public string Msg_Refund { get; set; }
        /// <summary>
        /// 新订单通知id
        /// </summary>
        public string WeChatAppId { get; set; }
        /// <summary>
        /// 公众号Secret
        /// </summary>
        public string WeChatSecret { get; set; }
        /// <summary>
        /// 腾讯地图开发者key
        /// </summary>
        public string MapApiKey { get; set; }
        /// <summary>
        /// 腾讯地图WebService接口Secret
        /// </summary>
        public string MapApiSecret { get; set; }
        /// <summary>
        /// 公众号管理后台地址
        /// </summary>
        public string MpUrl { get; set; }
        /// <summary>
        /// 顺丰同城开发者id
        /// </summary>
        public string ShunfengDevId { get; set; }
        /// <summary>
        /// 顺丰同城开发者密钥
        /// </summary>
        public string ShunfengDevKey { get; set; }
        /// <summary>
        /// 顺丰同城api地址
        /// </summary>
        public string ShunfengHost { get; set; }

        private string _elemeApi;
        /// <summary>
        /// 饿了么api请求地址
        /// </summary>
        public string ElemeApi
        {
            get
            {
                if (!string.IsNullOrEmpty(_elemeApi)) return _elemeApi;
                _elemeApi = RunMode == "product" ? "https://open-api.shop.ele.me/api/v1/" : "https://open-api-sandbox.shop.ele.me/api/v1/";
                return _elemeApi;
            }
        }
        private string _elemeToken;
        /// <summary>
        /// 饿了么token请求地址
        /// </summary>
        public string ElemeToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_elemeToken)) return _elemeToken;
                _elemeToken = RunMode == "product" ? "https://open-api.shop.ele.me/token" : "https://open-api-sandbox.shop.ele.me/token";
                return _elemeToken;
            }
        }

    }
}
