using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;

namespace JdCat.Cat.IRepository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        /// <summary>
        /// 创建新订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Order CreateOrder(Order order);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="status">订单状态</param>
        /// <param name="query">分页参数</param>
        /// <returns></returns>

        IEnumerable<Order> GetOrder(Business business, OrderStatus? status, PagingQuery query);
        /// <summary>
        /// 商户接单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool Receive(int orderId);
        /// <summary>
        /// 拒绝接单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="reason">拒绝原因</param>
        /// <returns></returns>
        bool Reject(int orderId, string reason);

    }
}
