using System;
using System.Collections.Generic;
using System.Text;

namespace JdCat.Cat.Common
{
    public class SfInputData
    {
        public string AppId { get; set; }
        public string AppKey { get; set; }

        private Dictionary<string, object> postData = new Dictionary<string, object>();
        public SfInputData(string appId, string appKey)
        {
            AppId = appId;
            AppKey = appKey;
        }


        /// <summary>
        /// 设置某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            postData[key] = value;
        }

        /// <summary>
        /// 制作签名
        /// </summary>
        /// <returns></returns>
        public string MakeSign()
        {
            var json = postData.ToJson();
            return Convert.ToBase64String($"{json}&{AppId}&{AppKey}".ToMd5().ToByte());
        }

        public string ToBody()
        {
            return postData.ToJson();
        }
        

    }
}
