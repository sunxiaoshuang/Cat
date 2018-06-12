using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JdCat.Cat.WsService.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetValue([FromQuery]string text, [FromServices]Dictionary<int, WebSocket> wsList)
        {
            foreach (var item in wsList)
            {
                await item.Value.SendAsync(System.Text.Encoding.Default.GetBytes(text), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
            return Ok(text);
        }
    }
}
