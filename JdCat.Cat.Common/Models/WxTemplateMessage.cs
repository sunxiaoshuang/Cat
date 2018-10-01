using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common.Models
{
    public class WxTemplateMessage
    {
        /// <summary>
        /// 访问Token
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string access_token { get; set; }
        /// <summary>
        /// 通知用户
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 使用的模版Id
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 跳转到小程序的页面
        /// </summary>
        public string page { get; set; }
        /// <summary>
        /// formid或者支付成功后的prepay_id
        /// </summary>
        public string form_id { get; set; }
        /// <summary>
        /// 模版内容：{ "keyword1": { "value": "8847893785"}, "keyword2": { "value": "2018年09月28日 22:10" } }
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 需要放大的关键词
        /// </summary>
        public string emphasis_keyword { get; set; }
    }
}
