﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.WxApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Text;
using JdCat.Cat.Common.Models;

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
        public async Task<IActionResult> Login(int businessId, [FromServices]ISessionDataRepository sessionService, [FromServices]IHostingEnvironment env)
        {
            var code = Request.Headers["X-WX-Code"];
            var business = Service.Set<Business>().First(a => a.ID == businessId);
            var session = await GetOpenId(code, business);
            if(string.IsNullOrEmpty(session.OpenId))
            {
                throw new Exception("未能得到用户OpenId，请检查小程序AppId与Secret配置");
            }
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
                    Userinfo = user,
                    Business = business
                }
            });
        }


        [HttpGet("business/{id}")]
        public IActionResult GetBusiness(int id)
        {
            var business = Service.WxGetBusiness(id);
            return Json(new
            {
                business.Address, business.AppId, business.Area, business.BusinessEndTime, business.BusinessEndTime2, business.BusinessEndTime3, business.BusinessLicenseImage, business.BusinessLicense, business.BusinessStartTime, business.BusinessStartTime2, business.BusinessStartTime3, business.Category, business.City, business.CityCode, business.CityName, business.Contact, business.Delivery, business.Description, business.DiscountQuantity, business.Distance, business.Freight, business.FreightMode, business.ID, business.IsClose, business.IsEnjoymentActivity, business.IsPublish, business.Lat, business.Lng, business.LogoSrc, business.MinAmount, business.Mobile, business.Name, business.Province, business.Range, business.Score, business.SpecialImage, business.TemplateNotifyId
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
        public IActionResult PutGrantPhone([FromBody]dynamic data)
        {
            var encrytedData = data.encrytedData.ToString();
            var iv = data.iv.ToString();
            var skey = int.Parse(Request.Headers["X-WX-Skey"]);
            var session = Service.Set<SessionData>().Find(skey);
            var sessionKey = session.SessionKey;
            var phoneMsg = UtilHelper.AESDecrypt(encrytedData, sessionKey, iv);
            var phone = JsonConvert.DeserializeObject<WxPhone>(phoneMsg);
            if (string.IsNullOrEmpty(phone.PurePhoneNumber))
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
        /// <summary>
        /// 新增地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <param name="sessionService"></param>
        /// <returns></returns>
        [HttpPost("address/{id}")]
        public IActionResult PostAddress(int id, [FromBody]Address address, [FromServices]ISessionDataRepository sessionService)
        {
            var result = new JsonData();
            var user = sessionService.GetSession(id).User;
            address.User = user;
            address.ModifyTime = DateTime.Now;
            Service.Set<Address>().Add(address);
            result.Success = Service.Commit() > 0;
            result.Msg = "ok";
            result.Data = address;
            return Json(result);
        }
        [HttpPost("createAddress")]
        public IActionResult CreateAddress([FromBody]Address address)
        {
            Service.Add(address);
            return Json(new JsonData { Data = address, Msg = "ok", Success = true });
        }
        /// <summary>
        /// 获取地址列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getAddress/{id}")]
        public IActionResult GetAddresses(int id)
        {
            var list = Service.GetAddresses(id);
            return Ok(list);
        }
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delAddress/{id}")]
        public IActionResult DelAddress(int id)
        {
            Service.DelAddress(id);
            return Ok("ok");
        }
        /// <summary>
        /// 获取用户地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("addressDetail/{id}")]
        public IActionResult AddressDetail(int id)
        {
            return Ok(Service.GetAddress(id));
        }
        /// <summary>
        /// 更新用户地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpPut("updateAddress/{id}")]
        public IActionResult UpdateDetail(int id, [FromBody]Address address)
        {
            var result = new JsonData();
            address.ID = id;
            result.Success = Service.UpdateAddress(address);
            result.Msg = "ok";
            result.Data = address;
            return Json(result);
        }
        /// <summary>
        /// 获取用户购物车
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        [HttpGet("carts/{id}")]
        public IActionResult GetCarts(int id)
        {
            return Json(Service.GetCarts(0, id));
        }

        /// <summary>
        /// 新增或更新用户购物车
        /// </summary>
        /// <param name="id"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        [HttpPost("carthandler")]
        public IActionResult PostCart([FromBody]ShoppingCart cart)
        {
            if (cart.ID == 0)
            {
                cart = Service.CreateCart(cart);
            }
            else
            {
                if (cart.Quantity > 0)
                {
                    cart = Service.UpdateCart(cart);
                }
                else
                {
                    Service.DeleteCart(cart);
                }
            }
            return Json(cart);
        }

        [HttpGet("updateCart/{id}")]
        public IActionResult UpdateCart(int id, [FromQuery]int quantity)
        {
            try
            {
                Service.UpdateCartQuantity(id, quantity);
            }
            catch (Exception ex)
            {
                Log.Error($"购物车更新异常：{ex.Message}");
                return Json(new JsonData { Success = false });
            }
            return Json(new JsonData { Success = true });
        }

        [HttpDelete("clearCart/{id}")]
        public IActionResult ClearCart(int id, [FromQuery]int businessId)
        {
            var result = new JsonData();
            result.Success = Service.ClearCart(id, businessId);
            if (!result.Success)
            {
                result.Msg = "购物车已经清空，请勿重复清理";
            }
            return Json(result);
        }

        [HttpGet("userCoupon/{id}")]
        public IActionResult GetUserCoupon(int id)
        {
            return Json(Service.GetUserCoupon(0, id));
        }
        [HttpPost("receiveCoupons/{id}")]
        public IActionResult ReceiveCoupons(int id, [FromBody]IEnumerable<SaleCouponUser> coupons)
        {
            var ids = coupons.Select(a => a.ID).ToArray();
            var list = Service.ReceiveCoupons(new User { ID = id }, ids);
            return Json(list);
        }

        [HttpPost("receiveReturnCoupon")]
        public async Task<IActionResult> ReceiveReturnCoupon ([FromBody]IEnumerable<SaleCouponUser> coupons, [FromServices]IUtilRepository util)
        {
            foreach (var item in coupons)
            {
                item.Code = await util.GetNextCodeForReturnCouponAsync();
            }
            await Service.ReceiveReturnCouponsAsync(coupons);
            return Json(new JsonData { Success = true, Data = coupons });
        }

        [HttpGet("comments/{id}")]
        public async Task<IActionResult> GetUserComments(int id)
        {
            var comments = await Service.GetUserComments(id);
            if(comments == null)
            {
                return Json("");
            }
            return Json(comments);
        }

        #region 私有方法

        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        private async Task<WxSession> GetOpenId(string code, Business business)
        {
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={business.AppId}&secret={business.Secret}&js_code={code}&grant_type=authorization_code";
            using (var hc = new HttpClient())
            {
                var response = await hc.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WxSession>(content);
            }
        }

        #endregion
    }
}
