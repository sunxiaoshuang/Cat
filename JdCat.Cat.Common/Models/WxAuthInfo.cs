using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    /// <summary>
    /// 第三方平台授权凭据和授权信息
    /// </summary>
    public class WxAuthInfo
    {
        public WxAuthInfo_Auth authorization_info { get; set; }

        public class WxAuthInfo_Auth
        {
            public string authorizer_access_token { get; set; }
            public string authorizer_appid { get; set; }
            public string authorizer_refresh_token { get; set; }
            public int expires_in { get; set; }
            public List<WxAuthInfo_Info> func_info { get; set; }
        }

        public class WxAuthInfo_Info
        {
            public WxAuthInfo_Info_Item funcscope_category { get; set; }
        }
        public class WxAuthInfo_Info_Item
        {
            private static Dictionary<int, string> dic = new Dictionary<int, string> {
                { 1, "消息管理权限" },
                { 2, "帐号服务权限"},
                { 3, "用户管理权限"},
                { 4, "网页服务权限"},
                { 5, "微信小店权限"},
                { 6, "微信多客服权限"},
                { 7, "群发与通知权限"},
                { 8, "微信卡券权限"},
                { 9, "微信扫一扫权限"},
                { 10, "微信连WIFI权限"},
                { 11, "素材管理权限"},
                { 12, "微信摇周边权限 "},
                { 13, "微信门店权限"},
                { 14, "微信支付权限"},
                { 15, "自定义菜单权限"},
                { 16, "获取认证状态及信息"},
                { 17, "帐号管理权限（小程序）"},
                { 18, "开发管理与数据分析权限（小程序）"},
                { 19, "客服消息管理权限（小程序）"},
                { 20, "微信登录权限（小程序）"},
                { 21, "数据分析权限（小程序）"},
                { 22, "城市服务接口权限 "},
                { 23, "广告管理权限"},
                { 24, "开放平台帐号管理权限"},
                { 25, "开放平台帐号管理权限（小程序）"},
                { 26, "微信电子发票权限"},
                { 41, "搜索widget的权限"}
            };
            public int id
            { get; set; }
            public string Name
            {
                get
                {
                    return dic[id];
                }
            }

        }
    }

}
