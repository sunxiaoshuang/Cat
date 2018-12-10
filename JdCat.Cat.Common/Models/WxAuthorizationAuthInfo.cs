using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 微信小程序授权信息类
    /// </summary>
    public class WxAuthorizationAuthInfo
    {
        public AuthorizerInfo authorizer_info { get; set; }

        public AuthorizationInfo authorization_info { get; set; }


        /// <summary>
        /// 授权帐号信息
        /// </summary>
        public class AuthorizerInfo
        {
            /// <summary>
            /// 授权方昵称
            /// </summary>
            public string nick_name { get; set; }
            /// <summary>
            /// 授权方头像
            /// </summary>
            public string head_img { get; set; }
            /// <summary>
            /// 授权方公众号类型，0代表订阅号，1代表由历史老帐号升级后的订阅号，2代表服务号，小程序默认为0
            /// </summary>
            public TypeInfo service_type_info { get; set; }
            /// <summary>
            /// 授权方认证类型，-1代表未认证，0代表微信认证，1代表新浪微博认证，2代表腾讯微博认证，3代表已资质认证通过但还未通过名称认证，4代表已资质认证通过、还未通过名称认证，但通过了新浪微博认证，5代表已资质认证通过、还未通过名称认证，但通过了腾讯微博认证
            /// </summary>
            public TypeInfo verify_type_info { get; set; }
            /// <summary>
            /// 授权方原始ID
            /// </summary>
            public string user_name { get; set; }
            /// <summary>
            /// 主体名称
            /// </summary>
            public string principal_name { get; set; }
            /// <summary>
            /// 用以了解以下功能的开通状况（0代表未开通，1代表已开通）： open_store:是否开通微信门店功能 open_scan:是否开通微信扫商品功能 open_pay:是否开通微信支付功能 open_card:是否开通微信卡券功能 open_shake:是否开通微信摇一摇功能
            /// </summary>
            public List<BusinessInfo> business_info { get; set; }
            /// <summary>
            /// 二维码图片的URL
            /// </summary>
            public string qrcode_url { get; set; }
            /// <summary>
            /// 授权方公众号所设置的微信号，可能为空
            /// </summary>
            public string alias { get; set; }
            /// <summary>
            /// 帐号介绍
            /// </summary>
            public string signature { get; set; }
            /// <summary>
            /// 小程序特有信息
            /// </summary>
            public List<MiniProgram> MiniProgramInfo { get; set; }

            public class TypeInfo
            {
                public int id { get; set; }
            }
            /// <summary>
            /// 帐号权限开通情况，0代表未开通，1代表已开通
            /// </summary>
            public class BusinessInfo
            {
                /// <summary>
                /// 微信门店功能
                /// </summary>
                public int open_store { get; set; }
                /// <summary>
                /// 微信扫商品功能
                /// </summary>
                public int open_scan { get; set; }
                /// <summary>
                /// 微信支付功能
                /// </summary>
                public int open_pay { get; set; }
                /// <summary>
                /// 微信卡券功能
                /// </summary>
                public int open_card { get; set; }
                /// <summary>
                /// 微信摇一摇功能
                /// </summary>
                public int open_shake { get; set; }
            }
            public class MiniProgram
            {
                public MiniProgramNetwork network { get; set; }
                public List<ServerCategory> categories { get; set; }
                public int visit_status { get; set; }
            }
            /// <summary>
            /// 小程序接口配置
            /// </summary>
            public class MiniProgramNetwork
            {
                public List<string> RequestDomain { get; set; }
                public List<string> WsRequestDomain { get; set; }
                public List<string> UploadDomain { get; set; }
                public List<string> DownloadDomain { get; set; }
            }
            /// <summary>
            /// 小程序服务类型
            /// </summary>
            public class ServerCategory
            {
                public string first { get; set; }
                public string second { get; set; }
            }

        }

        /// <summary>
        /// 授予的权限
        /// </summary>
        public class AuthorizationInfo
        {
            /// <summary>
            /// 授权方appid
            /// </summary>
            public string authorization_appid { get; set; }
            public List<FuncInfo> func_info { get; set; }

            /// <summary>
            /// 小程序授权给开发者的权限集列表，ID为17到19时分别代表： 17.帐号管理权限 18.开发管理权限 19.客服消息管理权限 请注意： 1）该字段的返回不会考虑小程序是否具备该权限集的权限（因为可能部分具备）。
            /// </summary>
            public class FuncInfo
            {
                public FuncscopeCategory funcscope_category { get; set; }
            }
            public class FuncscopeCategory
            {
                public int id { get; set; }
                public string name { get { return dic[id]; } }

                private static Dictionary<int, string> dic = new Dictionary<int, string>
                {
                    { 17, "消息管理权限" },
                    { 18, "帐号服务权限" },
                    { 19, "用户管理权限" }
                };
            }
        }
    }
}
