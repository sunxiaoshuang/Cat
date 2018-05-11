using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace JdCat.Cat.Common
{
    public class UtilHelper
    {
        public string GetMd5(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytResult = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult.ToLower();
        }
    }
}
