using System;
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
        /// 接收新订单
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

                //try
                //{
                //    using (var client = new HttpClient())
                //    {
                //        var req = client.GetAsync($"http://ws.whliupangzi.cn/api/notify/getorders/{id}");
                //        req.Wait();
                //        req.Result.EnsureSuccessStatusCode();
                //        var res = req.Result.Content.ReadAsStringAsync();
                //        res.Wait();
                //        return Content(res.Result);
                //    }
                //}
                //catch (Exception e)
                //{
                //    log.Error($"去读刘胖子订单出错：{e}");
                //}
            }
            else
            {
                result.Data = list.Select(a => a.Content).ToList();
            }
            return Json(result);
        }

    }
}
