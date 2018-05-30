using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model.Data;
using JdCat.Cat.WxApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController<IUserRepository, User>
    {
        public UserController(IUserRepository service) : base(service)
        {
        }
        [HttpGet]
        public async Task<IActionResult> Get(string code, int businessId)
        {
            var session = await GetOpenId(code, businessId);
            var user = Service.Get(session.OpenId);
            if (user == null)
            {
                user = new User { OpenId = session.OpenId, BusinessId = businessId };
                Service.Add(user);
            }
            user.SessionKey = session.Session_Key;
            LoginUser = user;
            return Ok(FilterUser(user));
        }

        [HttpPut("info")]
        public IActionResult PutGrantInfo([FromBody]User user)
        {
            var entity = Service.GrantInfo(user);
            return Ok(FilterUser(entity));
        }

        /// <summary>
        /// 过滤返回数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private object FilterUser(User user)
        {
            return new
            {
                user.ID,
                user.AvatarUrl,
                user.City,
                user.Country,
                user.Gender,
                user.Language,
                user.NickName,
                user.Province,
                user.Phone,
                user.IsRegister,
                user.IsPhone
            };
        }
        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        private async Task<WxSession> GetOpenId(string code, int businessId)
        {
            var business = Service.Set<Business>().First(a => a.ID == businessId);
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={business.AppId}&secret={business.Secret}&js_code={code}&grant_type=authorization_code";
            using (var hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<WxSession>(content);
            }
        }
    }
}
