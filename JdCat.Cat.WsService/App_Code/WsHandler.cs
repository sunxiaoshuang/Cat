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
        /// <summary>
        /// 网站端订单提醒
        /// </summary>
        public Dictionary<int, WebSocket> SocketDictionary { get; } = new Dictionary<int, WebSocket>();
        /// <summary>
        /// 客户端订单提醒
        /// </summary>
        public Dictionary<int, WebSocket> ClientSocketDictionary { get; } = new Dictionary<int, WebSocket>();

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
                var id = int.Parse(context.Request.Query["id"].FirstOrDefault());            // 获取商户id
                var type = context.Request.Query["type"].FirstOrDefault();
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    if (type == "client")
                    {
                        // 客户端连接
                        // 每个商户id只允许一个websocket连接
                        if (ClientSocketDictionary.ContainsKey(id))
                        {
                            var oldSocket = ClientSocketDictionary[id];
                            try
                            {
                                // 未知的原因会导致websocket对象释放掉，已释放的对象不用再次关闭
                                await oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "由于一个新的连接进入，您被迫下线了！", CancellationToken.None);
                            }
                            catch (Exception)
                            {

                            }
                            ClientSocketDictionary[id] = webSocket;
                        }
                        else
                        {
                            ClientSocketDictionary.Add(id, webSocket);
                        }
                        await Wait(context, webSocket, id);
                        ClientSocketDictionary.Remove(id);
                    }
                    else
                    {
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
                        SocketDictionary.Remove(id);
                    }
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
            var buffer = System.Text.Encoding.Default.GetBytes(code);
            // 发送客户端消息通知
            if (ClientSocketDictionary.ContainsKey(id))
            {
                var result = await OrderNotifyAsync(ClientSocketDictionary[id], buffer);
                if (!result)
                {
                    ClientSocketDictionary.Remove(id);
                }
                else
                {
                    // 如果客户端已经处理了通知，则不再发给网站处理
                    return;
                }
            }
            // 发送网站端消息通知
            if (SocketDictionary.ContainsKey(id))
            {
                var result = await OrderNotifyAsync(SocketDictionary[id], buffer);
                if(!result)
                {
                    SocketDictionary.Remove(id);
                }
            }
        }
        private async Task<bool> OrderNotifyAsync(WebSocket ws, byte[] buffer)
        {
            if (ws.CloseStatus.HasValue || ws.State == WebSocketState.Closed)
            {
                return false;
            }
            try
            {
                await ws.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Count()), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (WebSocketException)
            {
                return false;
            }
        }
    }
}
