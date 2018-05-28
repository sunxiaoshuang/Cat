using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Wx;
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
            var encryptedData = "i2TR4+KgTbx+5lHwk/gZxT5ZTP52jY+BVV+00M56TXt1nvolq1kkR7ZSfR36n9j8nTjaHO+lI/LakcfMUdb8LTSYkOwRc7TsTMEy44MgrnfJSvVd7iOY4ZObVDsrxcFGOafJCCwUrMdl7sM4RuednZKbaUaOG5EV6jbReq9LHLcEIJ1q2eaDncvJrlZSKGnLc03WaObzPeTCzurJHzxIlA==";
            var sessionKey = "EEgnA6Fe5D020K60CBWsJQ==";
            var appId = "wx7fc7dac038048c37";

            var iv = "lsO0TDfi1W3rTOnXzl5wvA==";
            var result = util.AESDecrypt(encryptedData, sessionKey, iv);
            var exist = result.Contains(appId);
            var user = JsonConvert.DeserializeObject<WxUser>(result);
            return Ok(result + "||appid:" + appId);
        }

    }
}