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
        //        [HttpGet]
        //        public async Task<IActionResult> Get(string code, int businessId, [FromServices]ISessionDataRepository sessionService)
        //        {
        //            var code = Request.Headers["X-WX-Code"];
        //            var session = await GetOpenId(code, businessId);
        //            var user = Service.Get(session.OpenId);
        //            if (user == null)
        //            {
        //                user = new User { OpenId = session.OpenId, BusinessId = businessId };
        //                Service.Add(user);
        //            }
        //            user.Skey = session.Session_Key;
        //            LoginUser = user;
        //            return Ok(FilterUser(user));
        //        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(int businessId, [FromServices]ISessionDataRepository sessionService)
        {
            var code = Request.Headers["X-WX-Code"];
            var session = await GetOpenId(code, businessId);
            var user = Service.Get(session.OpenId);
            if (user == null)
            {
                user = new User { OpenId = session.OpenId, BusinessId = businessId };
                Service.Add(user);
            }
            var sessData = sessionService.SetSession(new SessionData { SessionKey = session.Session_Key, UserId = user.ID });
            user.Skey = sessData.ID;
            return Ok(new WxRetInfo
            {
                Data = new WxUserData
                {
                    Skey = sessData.ID,
                    Userinfo = user
                }
            });
        }


        [HttpPut("info")]
        public IActionResult PutGrantInfo([FromBody]User user)
        {
            var entity = Service.GrantInfo(user);
            var skey = user.Skey;
            entity.Skey = skey;
            return Ok(new WxRetInfo
            {
                Data = new WxUserData
                {
                    Skey = skey,
                    Userinfo = entity
                }
            });
        }

        [HttpPut("phone")]
        public IActionResult PutGrantPhone([FromBody]dynamic data, [FromServices]UtilHelper util)
        {
            var encrytedData = data.encrytedData.ToString();
            var iv = data.iv.ToString();
            var skey = int.Parse(Request.Headers["X-WX-Skey"]);
            var session = Service.Set<SessionData>().Find(skey);
            var sessionKey = session.SessionKey;
            var phoneMsg = util.AESDecrypt(encrytedData, sessionKey, iv);
            var phone = JsonConvert.DeserializeObject<WxPhone>(phoneMsg);
            if(string.IsNullOrEmpty(phone.PurePhoneNumber))
            {
                return Ok(new WxRetInfo
                {
                    Code = -1,
                    Message = "微信未绑定手机号"
                });
            }
            Service.GrantPhone(session.UserId, phone.PurePhoneNumber);
            return Ok(new WxRetInfo
            {
                Data = phone.PurePhoneNumber
            });
        }

        [HttpPost("address/{id}")]
        public IActionResult PostAddress(int id, [FromBody]Address address, [FromServices]ISessionDataRepository sessionService)
        {
            var user = sessionService.Get(id).User;
            address.User = user;
            Service.Set<Address>().Add(address);
            Service.Commit();
            return Ok();
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
