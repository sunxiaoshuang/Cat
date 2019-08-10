using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JdCat.Cat.Common
{
    public class ElemeInputData
    {
        public string Secret { get; set; }
        public JObject Obj { get; set; }
        public ElemeInputData(JObject message, string secret)
        {
            this.Obj = message;
            this.Secret = secret;
        }

        /// <summary>
        /// 推送消息签名
        /// </summary>
        /// <returns></returns>
        public string MakeSign()
        {
            var sort = new SortedDictionary<string, object>();
            foreach (var item in Obj.Properties())
            {
                if (item.Name == "signature") continue;
                sort.Add(item.Name, item.Value);
            }
            var arr = sort.Select(a => a.Key + "=" + a.Value).ToList();
            var sb = new StringBuilder();
            arr.ForEach(a => sb.Append(a));
            sb.Append(Secret);
            var sign = sb.ToString().ToMd5().ToUpper();
            return sign;
        }

        /// <summary>
        /// api接口签名
        /// </summary>
        /// <returns></returns>
        public string MakeSignApi()
        {
            var metas = Obj["metas"].Value<JObject>();
            var @params = Obj["params"].Value<JObject>();
            var sort = new SortedDictionary<string, object>();
            metas.Properties().ForEach(a => sort[a.Name] = metas[a.Name]);
            @params.Properties().ForEach(a => sort[a.Name] = @params[a.Name]);
            var arr = sort.Select(a => a.Key + "=" + a.Value.ToJson()).ToList();

            var action = Obj["action"].Value<string>();
            var token = Obj["token"].Value<string>();
            var sb = new StringBuilder();
            arr.ForEach(a => sb.Append(a));
            var str = action + token + sb + Secret;
            var sign = str.ToMd5().ToUpper();
            return sign;
        }

        /// <summary>
        /// 验证推送消息签名是否正确
        /// </summary>
        /// <returns></returns>
        public bool CheckSign()
        {
            var sign = MakeSign();
            return Obj["signature"].Value<string>() == sign;
        }

    }
}
