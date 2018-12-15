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
    public class InputData
    {
        public const string SIGN_TYPE_MD5 = "MD5";
        public const string SIGN_TYPE_HMAC_SHA256 = "HMAC-SHA256";

        /// <summary>
        /// 采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        /// </summary>
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /// <summary>
        /// 商户平台密钥key
        /// </summary>
        public static string Key { get; set; }

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
        /// 将Dictionary转成xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            if (0 == m_values.Count)
            {
                throw new ArgumentNullException("字典数据为空，不能转化为xml");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new ArgumentNullException("字典内部存在null值");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else
                {
                    throw new ArgumentException("字典内部的值只能是string或int类型");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// 将xml转为WxPayData对象并返回对象内部的数据
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public SortedDictionary<string, object> FromXml(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }

            if (m_values["return_code"] + "" != "SUCCESS")
            {
                return m_values;
            }
            CheckSign();
            return m_values;
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

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// Dictionary格式化成Json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(m_values);
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
        public string MakeSign(string signType = "MD5")
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += $"&key={Key}";
            if (signType == SIGN_TYPE_MD5)
            {
                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }
            else if (signType == SIGN_TYPE_HMAC_SHA256)
            {
                return CalcHMACSHA256Hash(str);
            }
            else
            {
                throw new Exception("sign_type 不合法");
            }
        }

        /// <summary>
        /// 检测签名是否正确
        /// </summary>
        /// <param name="signType"></param>
        /// <returns></returns>
        public void CheckSign(string signType = "MD5")
        {
            var return_sign = GetValue("sign").ToString();

            var cal_sign = MakeSign(signType);

            if (cal_sign == return_sign)
            {
                return;
            }
            throw new Exception("微信接口签名不匹配");
        }
        
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }

        private string CalcHMACSHA256Hash(string plaintext)
        {
            var result = "";
            var enc = Encoding.Default;
            var baText2BeHashed = enc.GetBytes(plaintext);
            var baSalt = enc.GetBytes(Key);
            var hasher = new HMACSHA256(baSalt);
            var baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
        

    }
}
