﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.WxApi.Controllers
{
    [Route("api/[controller]")]
    public class BusinessController : BaseController<IBusinessRepository, Business>
    {
        public BusinessController(IBusinessRepository service) : base(service)
        {

        }

        [HttpGet("{id}")]
        public IActionResult GetBusiness(int id)
        {
            return Json(Service.Get(a => a.ID == id));
        }

        /// <summary>
        /// 进入小程序时，初始化各种数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("init/{id}")]
        public IActionResult WxPageInit(int id, int userId, [FromServices]IUserRepository userRepository)
        {
            var now = DateTime.Now;
            // 满减活动
            var fullReduct = Service.GetFullReduce(new Business { ID = id }, false).ToList();
            var valid = fullReduct.Where(a => a.IsActiveValid());
            // 优惠券
            var coupon = Service.GetCouponValid(new Business { ID = id });
            // 折扣券
            var discount = Service.GetDiscounts(new Business { ID = id })
                .Where(a => a.Status == ActivityStatus.Active && a.StartDate <= now && a.EndDate >= now).ToList();
            // 用户优惠券
            var userCoupon = userRepository.GetUserCoupon(id, userId);
            // 用户用户
            // 商户配送费用设置
            var freights = Service.GetFreights(id);
            // 用户购物车
            var carts = userRepository.GetCarts(id, userId);

            return Json(new { fullReduct = valid, coupon, discount, userCoupon, freights, carts });

        }

        /// <summary>
        /// 进入小程序时，初始化各种数据（删除用户购物车）
        /// </summary>
        /// <returns></returns>
        [HttpGet("init2/{id}")]
        public async Task<IActionResult> WxPageInit2(int id, int userId, [FromServices]IUserRepository userRepository)
        {
            var now = DateTime.Now;
            // 用户信息
            var user = await userRepository.GetAsync<User>(userId);
            // 满减活动
            var fullReduct = Service.GetFullReduce(new Business { ID = id }, false).ToList();
            var valid = fullReduct.Where(a => a.IsActiveValid());
            // 优惠券
            var coupon = Service.GetCouponValid(new Business { ID = id });
            // 消费返券
            var retCoupon = (await Service.GetBusinessReturnCouponAsync(id)).Where(a => a.Status == CouponStatus.Up);
            // 折扣券
            var discount = Service.GetDiscounts(new Business { ID = id })
                .Where(a => a.Status == ActivityStatus.Active && a.StartDate <= now && a.EndDate >= now).ToList();
            // 新客立减
            var newCustom = await Service.GetBusinessNewCustomAsync(id);
            if (newCustom != null && (newCustom.StartTime > now || newCustom.EndTime.AddDays(1) < now))
            {
                newCustom = null;
            }
            // 用户优惠券
            var userCoupon = userRepository.GetUserCoupon(id, userId);
            // 商户配送费用设置
            var freights = Service.GetFreights(id);

            return Json(new { fullReduct = valid, coupon, retCoupon, discount, userCoupon, freights, newCustom, user });

        }

        [HttpGet("fullreduce/{id}")]
        public IActionResult GetFullReduce(int id)
        {
            var list = Service.GetFullReduce(new Business { ID = id }, false).ToList();
            var valid = list.Where(a => a.IsActiveValid());
            return Json(valid);
        }

        [HttpGet("coupon/{id}")]
        public IActionResult GetCoupon(int id)
        {
            return Json(Service.GetCouponValid(new Business { ID = id }));
        }

        /// <summary>
        /// 获取商户营销活动与用户优惠券
        /// </summary>
        /// <returns></returns>
        [HttpGet("sale/{id}")]
        public IActionResult GetSale(int id, int userId, [FromServices]IUserRepository userRepository)
        {
            var now = DateTime.Now;
            // 满减活动
            var fullReduct = Service.GetFullReduce(new Business { ID = id }, false).ToList();
            var valid = fullReduct.Where(a => a.IsActiveValid());
            // 优惠券
            var coupon = Service.GetCouponValid(new Business { ID = id });
            // 折扣券
            var discount = Service.GetDiscounts(new Business { ID = id })
                .Where(a => a.Status == ActivityStatus.Active && a.StartDate <= now && a.EndDate >= now).ToList();

            // 用户优惠券
            var userCoupon = userRepository.GetUserCoupon(id, userId);

            return Json(new { fullReduct = valid, coupon, discount, userCoupon });
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery]string username, [FromQuery]string pwd)
        {
            var business = Service.Login(username, UtilHelper.MD5Encrypt(pwd));
            var result = new JsonData();
            if (business == null)
            {
                result.Msg = "用户名或密码错误";
            }
            else
            {
                result.Data = business;
                result.Success = true;
            }
            return Json(result);
        }

        [HttpGet("getComments/{id}")]
        public IActionResult GetComments(int id, [FromQuery]PagingQuery paging)
        {
            var list = Service.GetComments(id, paging);
            return Json(new
            {
                list,
                more = list.Count > 0
            });
        }

    }
}
