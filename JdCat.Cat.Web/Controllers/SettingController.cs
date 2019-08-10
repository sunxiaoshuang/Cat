using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.Web.Controllers
{
    public class SettingController : BaseController<IUtilRepository, Business>
    {
        public SettingController(AppData appData, IUtilRepository service) : base(appData, service)
        {
        }
        /// <summary>
        /// 重定向到授权页
        /// </summary>
        /// <param name="appData"></param>
        /// <returns></returns>
        public async Task<IActionResult> AuthPage([FromServices]AppData appData)
        {
            var token = await Service.GetOpenTokenAsync();
            var preCode = await WxHelper.GetOpenPreAuthCodeAsync(token);
            var url = $"https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid={WxHelper.OpenAppId}&pre_auth_code={preCode}&redirect_uri={appData.Domain}/Setting/AuthSuccess&auth_type=3";
            return Redirect(url);
        }
        /// <summary>
        /// 授权成功后的回调URL
        /// </summary>
        /// <param name="auth_code"></param>
        /// <param name="expires_in"></param>
        /// <returns></returns>
        public async Task<IActionResult> AuthSuccess([FromQuery]string auth_code, [FromQuery]int expires_in, [FromServices]AppData appData)
        {
            var token = await Service.GetOpenTokenAsync();
            var result = await WxHelper.GetAuthTokenAsync(auth_code, token);
            var openAuth = new OpenAuthInfo
            {
                AppId = result.authorization_info.authorizer_appid,
                BusinessId = Business.ID,
                CreateTime = DateTime.Now,
                RefreshToken = result.authorization_info.authorizer_refresh_token
            };
            await Service.SaveAuthorizerInfoAsync(openAuth);
            return RedirectToAction("AuthInfo");
        }

        /// <summary>
        /// 小程序管理
        /// </summary>
        /// <returns></returns>
        public IActionResult AuthInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAuthInfo()
        {
            //var authInfo = Service.GetAuthInfoByBusinessIdAsync(Business.ID);
            var token = await Service.GetAuthorizerAccessTokenAsync(Business.WeChatAppId);
            var info = await WxHelper.GetAuthorizerInfoAsync(token, Business.WeChatAppId);
            return Content(info);
        }

        /// <summary>
        /// 权限信息页
        /// </summary>
        /// <returns></returns>
        public IActionResult AuthInfoPage()
        {
            return View();
        }

    }
}
