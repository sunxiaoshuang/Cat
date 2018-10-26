using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.Repository.Model
{
    public class FeyinModel
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int? ErrCode { get; set; }
        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 身份令牌
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 过期时长
        /// </summary>
        public int? Expires_In { get; set; }
    }
}
