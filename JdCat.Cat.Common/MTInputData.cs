using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace JdCat.Cat.Common
{
    public class MTInputData
    {
        public MTInputData(string key, string url)
        {
            this.Key = key;
            this.Url = url;
        }

        /// <summary>
        /// 采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        /// </summary>
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
        
        public string Key { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 设置某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /// <summary>
        /// 根据字段名获取某个字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            m_values.TryGetValue(key, out object o);
            return o;
        }

        /// <summary>
        /// 判断某个字段是否已设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            m_values.TryGetValue(key, out object o);
            if (null != o)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Dictionary格式转化成url参数格式
        /// </summary>
        /// <returns></returns>
        public string ToUrl()
        {
            var buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new ArgumentNullException("字典内部存在null值");
                }

                if (pair.Key != "sign")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        /// </summary>
        /// <returns></returns>
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new ArgumentNullException("字典内部存在null值");
                }
                str += $"{pair.Key}={pair.Value}\n";
            }
            str = HttpUtility.HtmlEncode(str);
            return str;
        }

        /// <summary>
        /// 生成签名，详见签名生成算法
        /// </summary>
        /// <param name="signType"></param>
        /// <returns></returns>
        public string MakeSign()
        {
            string str = ToUrl();
            str = $"{Url}?{str}{Key}";
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }


    }
}
