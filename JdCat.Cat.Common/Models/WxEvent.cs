using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    public class WxEvent
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// OpenID
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string Event { get; set; }
        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string EventKey { get; set; }
        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; set; }
    }
}
