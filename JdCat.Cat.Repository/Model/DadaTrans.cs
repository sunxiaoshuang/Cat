using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace JdCat.Cat.Repository.Model
{
    public class DadaTrans
    {
        public string App_key { get; set; }
        public string App_secret { get; set; }
        public string Signature { get; set; }
        public long Timestamp { get; set; }
        public string Format { get; set; } = "json";
        public string V { get; set; } = "1.0";
        public string Source_id { get; set; }
        public string Body { get; set; } = "";

        public void Generator()
        {
            var str = $"{App_secret}app_key{App_key}body{Body}format{Format}source_id{Source_id}timestamp{Timestamp}v{V}{App_secret}";
            Signature = MD5Encrypt(str).ToUpper();
        }
        private string MD5Encrypt(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytResult = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult.ToLower();
        }
    }
}
