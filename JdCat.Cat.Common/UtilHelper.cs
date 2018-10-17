﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using ZXing.Common;
using ZXing;

namespace JdCat.Cat.Common
{
    public class UtilHelper
    {
        static UtilHelper()
        {
            var dirLogInfo = Path.Combine(Directory.GetCurrentDirectory(), "Log", "Info");
            if (!Directory.Exists(dirLogInfo))
            {
                Directory.CreateDirectory(dirLogInfo);
            }
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var t = (time.Ticks - startTime.Ticks) / 10000;
            return t;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name="timeStamp"></param>
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = (long.Parse(timeStamp) * 10000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        #region 加密与解密

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt(string input)
        {
            return MD5Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string MD5Encrypt(string input, Encoding encode)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytResult = md5.ComputeHash(encode.GetBytes(input));
            var strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult.ToLower();
        }

        /// <summary>
        /// AES-128-CBC
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(string input, string key, string iv)
        {
            var encryptedData = Convert.FromBase64String(input);
            var rijndaelCipher = new RijndaelManaged
            {
                Key = Convert.FromBase64String(key),
                IV = Convert.FromBase64String(iv),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
            var transform = rijndaelCipher.CreateDecryptor();
            var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            var result = Encoding.UTF8.GetString(plainText);
            return result;
        }
        
        public static string SHA1(string content, Encoding encode = null)
        {
            if (encode == null)
            {
                encode = Encoding.UTF8;
            }
            try
            {
                var sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                var enText = new StringBuilder();
                foreach (byte iByte in bytes_out)
                {
                    enText.AppendFormat("{0:x2}", iByte);
                }
                return enText.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
        
        #endregion

        /// <summary>
        /// 将对象格式化为xml流媒体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="o"></param>
        public static void XmlSerializeInternal(Stream stream, object o)
        {
            if (o == null) throw new ArgumentNullException("o");
            var serializer = new XmlSerializer(o.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n",
                Encoding = Encoding.UTF8,
                IndentChars = " "
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        private static Dictionary<Type, IEnumerable<PropertyInfo>> dicProperty = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        /// <summary>
        /// 读取xml字符串，返回指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ReadXml<T>(string xml) where T : class, new()
        {
            var document = XDocument.Parse(xml);
            var root = document.Root;
            var type = typeof(T);
            var entity = new T();
            IEnumerable<PropertyInfo> properties = null;
            if (dicProperty.ContainsKey(type))
            {
                properties = dicProperty[type];
            }
            else
            {
                properties = type.GetProperties();
                dicProperty.Add(type, properties);
            }
            foreach (var item in root.Elements())
            {
                var property = properties.FirstOrDefault(a => a.Name == item.Name);
                if (property == null) continue;
                property.SetValue(entity, item.Value);
            }
            return entity;
        }

        /// <summary>
        /// 记录系统日志，路径：Log/Info
        /// </summary>
        /// <param name="content"></param>
        public static void Log(string content)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Log", "Info", DateTime.Now.ToString("yyyyMMdd") + ".txt");
            if (!File.Exists(filepath))
            {
                File.Create(filepath).Close();
            }
            content += "\r\n " + Environment.NewLine;
            File.AppendAllText(filepath, content);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt.Ticks) / 10000;
        }

        #region 验证码

        private static int letterWidth = 18;//单个字体的宽度范围
        private static int letterHeight = 28;//单个字体的高度范围
        private static char[] chars = "0123456789".ToCharArray();
        private static string[] fonts = { "Arial", "Georgia" };
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] GenerateVerificationCode(string code)
        {
            int int_ImageWidth = code.Length * letterWidth;
            var newRandom = new Random();
            var image = new Bitmap(int_ImageWidth, letterHeight);
            var g = Graphics.FromImage(image);
            //生成随机生成器
            var random = new Random();
            //白色背景
            g.Clear(Color.White);
            //画图片的背景噪音线
            for (int i = 0; i < 10; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            //画图片的前景噪音点
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);

                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            //随机字体和颜色的验证码字符

            int findex;
            for (int int_index = 0; int_index < code.Length; int_index++)
            {
                findex = newRandom.Next(fonts.Length - 1);
                string str_char = code.Substring(int_index, 1);
                Brush newBrush = new SolidBrush(GetRandomColor());
                Point thePos = new Point(int_index * letterWidth + 1 + newRandom.Next(3), 1 + newRandom.Next(3));//5+1+a+s+p+x
                g.DrawString(str_char, new Font(fonts[findex], 12, FontStyle.Bold), newBrush, thePos);
            }
            //灰色边框
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, int_ImageWidth - 1, (letterHeight - 1));
            //图片扭曲
            //image = TwistImage(image, true, 3, 4);
            //将生成的图片发回客户端
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                image.Dispose();
                return ms.ToArray();
            }
        }

        private static Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(210);
            int int_Green = RandomNum_Sencond.Next(180);
            int int_Blue = (int_Red + int_Green > 300) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return Color.FromArgb(int_Red, int_Green, int_Blue);// 5+1+a+s+p+x
        }

        #endregion

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="gifFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static byte[] CreateCodeEwm(string message, int width = 400, int height = 400)
        {
            int heig = width;
            if (width > height)
            {
                heig = height;
                width = height;
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }
            var w = new ZXing.QrCode.QRCodeWriter();

            BitMatrix b = w.encode(message, BarcodeFormat.QR_CODE, width, heig);
            var zzb = new ZXing.ZKWeb.BarcodeWriter
            {
                Options = new EncodingOptions { Margin = 0 }
            };
            Bitmap bmp = zzb.Write(b);
            byte[] byteArray = null;
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                byteArray = stream.GetBuffer();
            }
            bmp.Dispose();
            return byteArray;
        }
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string RandNum(int len = 6)
        {
            var arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var num = new StringBuilder();
            var rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < len; i++)
            {
                num.Append(arrChar[rnd.Next(0, 9)].ToString());
            }
            return num.ToString();
        }

        


    }
}
