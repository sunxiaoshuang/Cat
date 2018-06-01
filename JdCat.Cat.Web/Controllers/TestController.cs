using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.WxApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JdCat.Cat.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Decrypted([FromServices]UtilHelper util)
        {
            var encryptedData = "KB2Wfoq7RKPjGsX3z8uO2XTzKbVqw7Sso/vzFlrLWmchnvHt21+SfEUUchwd1408XBNGhzSCDh7yyTu+9a5tgCxrU8nzOHji7o8k1ZjNiaV7DrZOEtzvOZmQUuDk16ozJtXb11C1slU5ATwXwtwEMGwmGg1RaJ6ux7GH4AJeAwHtcJjAaqbJQ2s6j8iKUM3BmP2/VDCr/Ioh2n6LJS37ww==";
            var sessionKey = "Vb9X2sRS8++jD/q9MPRPDw==";
            var appId = "wx7fc7dac038048c37";

            var iv = "QGtlrCecZJtuEc1HaVI4Eg==";
            var result = util.AESDecrypt(encryptedData, sessionKey, iv);
            var exist = result.Contains(appId);
            var user = JsonConvert.DeserializeObject<WxUser>(result);
            return Ok(result + "||appid:" + appId);
        }

    }
}