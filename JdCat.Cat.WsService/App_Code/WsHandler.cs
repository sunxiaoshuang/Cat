using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace JdCat.Cat.WsService.App_Code
{
    /// <summary>
    /// 处理WebSocket连接的类
    /// </summary>
    public class WsHandler
    {
        public Dictionary<int, WebSocket> SocketDictionary { get; } = new Dictionary<int, WebSocket>();
        private byte[] msg = System.Text.Encoding.Default.GetBytes("新订单提醒");

        /// <summary>
        /// 处理websocket请求
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="next">中间件之后执行</param>
        /// <returns></returns>
        public async Task WebSocktHandlerAsync(HttpContext context, Func<Task> next)
        {
            if (context.Request.Path.Value.Contains("/ws"))
            {
                var id = int.Parse(context.Request.Query["id"]);            // 获取商户id
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    // 每个商户id只允许一个websocket连接
                    if (SocketDictionary.ContainsKey(id))
                    {
                        var oldSocket = SocketDictionary[id];
                        try
                        {
                            // 未知的原因会导致websocket对象释放掉，已释放的对象不用再次关闭
                            await oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "由于一个新的连接进入，您被迫下线了！", CancellationToken.None);
                        }
                        catch (Exception)
                        {

                        }
                        SocketDictionary[id] = webSocket;
                    }
                    else
                    {
                        SocketDictionary.Add(id, webSocket);
                    }
                    await Wait(context, webSocket, id);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
        }
        /// <summary>
        /// 保持连接状态，接收客户端发来的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task Wait(HttpContext context, WebSocket webSocket, int id)
        {
            var buffer = new byte[1024 * 1];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            // 接收客户端消息，直到拿到关闭消息为止
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            SocketDictionary.Remove(id);
        }
        /// <summary>
        /// 发送新订单任务提醒
        /// </summary>
        /// <param name="id">商户id</param>
        /// <param name="code">订单编号</param>
        public async Task OrderNotifyAsync(int id, string code)
        {
            if (!SocketDictionary.ContainsKey(id)) return;
            var ws = SocketDictionary[id];
            if (ws.CloseStatus.HasValue || ws.State == WebSocketState.Closed)
            {
                SocketDictionary.Remove(id);
                return;
            }
            try
            {
                var content = System.Text.Encoding.Default.GetBytes(code);
                await ws.SendAsync(new ArraySegment<byte>(content, 0, content.Count()), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                // 未知的原因会导致websocket对象释放掉，所以捕获到该异常时，直接将对象清除掉
                SocketDictionary.Remove(id);
            }
        }
    }
}
