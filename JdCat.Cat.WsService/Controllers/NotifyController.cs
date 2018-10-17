using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.WsService.App_Code;
using JdCat.Cat.WsService.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;

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
            await wsHandler.OrderNotifyAsync(id, code, state??99);
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

    }
}
