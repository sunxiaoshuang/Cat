using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JdCat.Cat.WsService.App_Code
{
    public class OrderInfoHandler
    {
        protected OrderInfoHandler()
        {

        }
        private static OrderInfoHandler _info;
        /// <summary>
        /// 获取处理类
        /// </summary>
        /// <returns></returns>
        public static OrderInfoHandler GetHandler()
        {
            if (_info == null)
            {
                _info = new OrderInfoHandler();
            }
            return _info;
        }

        private Dictionary<int, List<PostData>> _notifyData;

        /// <summary>
        /// 添加订单数据
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public bool Add(PostData postData)
        {
            if (postData.BusinessId == 0 || postData.OrderId == 0) return false;
            List<PostData> list;
            if (_notifyData.ContainsKey(postData.BusinessId))
            {
                list = _notifyData[postData.BusinessId];
            }
            else
            {
                list = new List<PostData>();
                _notifyData.Add(postData.BusinessId, list);
            }
            RemoveTimeoutData(list);
            var data = list.SingleOrDefault(a => a.OrderId == postData.OrderId);
            if (data != null) return false;
            postData.ReceviceTime = DateTime.Now;
            list.Add(postData);
            return true;
        }

        public List<PostData> Get(int businessId)
        {
            if (!_notifyData.ContainsKey(businessId)) return null;
            var list = _notifyData[businessId];
            return list;
        }

        private int TimeoutSecond = 60 * 60 * 3;
        /// <summary>
        /// 移除超时的订单，默认超过3小时的订单为超时的订单
        /// </summary>
        private void RemoveTimeoutData(List<PostData> list)
        {
            if (list.Count == 0) return;
            var now = DateTime.Now;
            list.RemoveAll(a => (now - a.ReceviceTime).TotalSeconds > TimeoutSecond);
        }

    }
}
