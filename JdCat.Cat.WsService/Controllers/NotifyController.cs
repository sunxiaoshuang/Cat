﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.WsService.App_Code;
using JdCat.Cat.WsService.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace JdCat.Cat.WsService.Controllers
{
    [Route("api/[controller]")]
    public class NotifyController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(NotifyController));
        public ILog Log
        {
            get
            {
                return log;
            }
        }
        /// <summary>
        /// 获取新订单
        /// </summary>
        /// <param name="id">商户id</param>
        /// <param name="wsList"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery]string code, [FromQuery]int? state, [FromServices]WsHandler wsHandler)
        {
            // 网页通知
            await wsHandler.OrderNotifyAsync(id, code, state ?? 99);
            // 客户端通知
            //var state = StateObject.DicSocket.FirstOrDefault(a => a.Key == id);
            //if (state.Value != null)
            //{
            //    if (!state.Value.workSocket.Poll(10, System.Net.Sockets.SelectMode.SelectRead))
            //    {
            //        AsynchronousSocketListener.Send(state.Value, code);
            //    }
            //    else
            //    {
            //        StateObject.DicSocket.Remove(id);
            //    }
            //}
            return Ok("新订单通知成功");
        }

        /// <summary>
        /// 接收新订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]PostNewOrderData data, [FromServices]OrderInfoHandler handler)
        {
            handler.Add(data);
            return Ok("通知成功");
        }

        /// <summary>
        /// 根据商户id获取自己的订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetOrders/{id}")]
        public IActionResult Get(int id, [FromServices]OrderInfoHandler handler)
        {
            var result = new JsonData { Success = true };
            var list = handler.Get(id);
            if (list == null || list.Count == 0)
            {
                result.Msg = "没有新订单";
            }
            else
            {
                result.Data = list.Select(a => a.Content).ToList();
            }
            return Json(result);
        }

        /// <summary>
        /// 获取商户第三方订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("thirdOrder/{id}")]
        public async Task<IActionResult> GetThirdOrder(int id, [FromServices]IConnectionMultiplexer connection)
        {
            var json = new JsonData { Success = true };
            var database = connection.GetDatabase();
            var result = await database.ScriptEvaluateAsync(LuaScript.Prepare(@"
                local res = redis.call('KEYS', @keypattern) 
                return res"), new { keypattern = $"Jiandanmao:Notify:ThirdOrder:{id}:*" });

            if (result.IsNull)
            {
                return Json(json);
            }

            var keys = (RedisKey[])result;
            var vals = await database.StringGetAsync(keys);
            if (vals.Length == 0) return Json(json);
            var now = DateTime.Now;
            var ids = new List<RedisKey>();
            json.Data = vals.Where(a =>
            {
                var obj = JObject.Parse(a);
                var time = obj["deliveryTime"].Value<DateTime?>();
                if (time != null && time > now.AddHours(1)) return false;
                ids.Add($"Jiandanmao:Notify:ThirdOrder:{id}:{obj["id"].Value<int>()}");
                return true;
            }).ToList();

            await database.KeyDeleteAsync(ids.ToArray());

            return Json(json);
        }


        /// <summary>
        /// 获取商户订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrders(int id, [FromServices]IConnectionMultiplexer connection)
        {
            var json = new JsonData { Success = true };
            database = connection.GetDatabase();

            json.Data = new
            {
                local = await GetLocalOrdersAsync(id),
                third = await GetThirdOrdersAsync(id),
            };

            return Json(json);
        }

        private IDatabase database;
        /// <summary>
        /// 获取商户的第三方订单通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<RedisValue>> GetThirdOrdersAsync(int id)
        {
            var result = await database.ScriptEvaluateAsync(LuaScript.Prepare(@"
                local res = redis.call('KEYS', @keypattern) 
                return res"), new { keypattern = $"Jiandanmao:Notify:ThirdOrder:{id}:*" });
            if (result.IsNull) return null;
            var keys = (RedisKey[])result;
            var vals = await database.StringGetAsync(keys);
            if (vals.Length == 0) return null;
            var now = DateTime.Now;
            var ids = new List<RedisKey>();

            //DateTime? time;
            //var list = vals.Where(a =>
            //{
            //    var obj = JObject.Parse(a);
            //    // 如果存在预约时间，且预约时间大于当前时间一小时，则暂不发出通知
            //    if (obj["deliveryTime"] != null
            //        && (time = obj["deliveryTime"].Value<DateTime?>()) != null
            //        && time > now.AddHours(1))
            //    {
            //        return false;
            //    }
            //    ids.Add($"Jiandanmao:Notify:ThirdOrder:{id}:{obj["id"].Value<int>()}");
            //    return true;
            //}).ToList();

            //await database.KeyDeleteAsync(ids.ToArray());
            //return list;

            #region 包含预订单逻辑

            var results = new List<RedisValue>();
            vals.ForEach(a =>
            {
                var obj = JObject.Parse(a);                                                 // 订单对象
                var time = obj["deliveryTime"].Value<DateTime?>();                          // 预约时间
                var key = $"Jiandanmao:Notify:ThirdOrder:{id}:{obj["id"].Value<int>()}";    // 订单key
                if (time == null || time < now.AddMinutes(50)) // 如果预约时间为空或者距离当前时间小于50分钟，则直接输出
                {
                    results.Add(a);
                    ids.Add(key);
                    return;
                }
                // 预订单则需要判断前台打印与后台打印逻辑
                var mode = obj["printType"].Value<int>();
                if (mode == 3)                  // 仅考虑整单打印的情况
                {
                    obj["printType"] = 1;       // 将订单打印方式改为打印前台小票
                    results.Add(JsonConvert.SerializeObject(obj));      // 添加到输出结果
                    obj["printType"] = 2;       // 然后将打印方式改为打印后台小票
                    // 重新设置打印任务
                    var timespan = time.Value.AddHours(2) - now;
                    database.StringSetAsync(key, JsonConvert.SerializeObject(obj), timespan);
                }
            });
            await database.KeyDeleteAsync(ids.ToArray());
            return results;
            #endregion


            //var results = new List<RedisValue>();
            //vals.ForEach(a =>
            //{
            //    var obj = JObject.Parse(a);                                                 // 订单对象
            //    var key = $"Jiandanmao:Notify:ThirdOrder:{id}:{obj["id"].Value<int>()}";    // 订单key
            //    results.Add(a);
            //    ids.Add(key);
            //});

            //await database.KeyDeleteAsync(ids.ToArray());
            //return results;
        }
        /// <summary>
        /// 获取商户本地订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<RedisValue[]> GetLocalOrdersAsync(int id)
        {
            var result = await database.ScriptEvaluateAsync(LuaScript.Prepare(@"
                local res = redis.call('KEYS', @keypattern) 
                return res"), new { keypattern = $"Jiandanmao:Notify:Order:{id}:*" });
            if (result.IsNull) return null;
            var keys = (RedisKey[])result;
            var vals = await database.StringGetAsync(keys);
            if (vals.Length == 0) return null;
            await database.KeyDeleteAsync(keys);
            return vals;
        }

    }
}
