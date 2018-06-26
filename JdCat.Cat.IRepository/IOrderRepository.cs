using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;

namespace JdCat.Cat.IRepository
{
    public interface IOrderRepository : IBusinessRepository<Order>
    {
        Order Get(int id);
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
        /// <param name="code">订单编号</param>
        /// <param name="phone">用户手机号</param>
        /// <returns></returns>
        IEnumerable<Order> GetOrder(Business business, OrderStatus? status, PagingQuery query, string code, string phone);
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
        /// <summary>
        /// 自己配送订单
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        bool SendOrderSelf(int id);
        /// <summary>
        /// 订单送达
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        bool Achieve(int id);
        /// <summary>
        /// 根据达达的回调，更新订单的状态
        /// </summary>
        /// <param name="dada"></param>
        void UpdateOrderStatus(DadaCallBack dada);
        /// <summary>
        /// 达达配送接口调用成功后保存订单状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="back"></param>
        bool SendSuccess(Order order, DadaResult<DadaReturn> back);
        /// <summary>
        /// 达达配送取消后，保存订单状态
        /// </summary>
        /// <param name="order"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        bool CancelSuccess(Order order, DadaResult<DadaLiquidatedDamages> back);
        /// <summary>
        /// 获取商户绑定的打印机
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<FeyinDevice> GetPrinters(Business business);
        /// <summary>
        /// 获取订单，包含产品集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order GetOrderIncludeProduct(int id);
    }
}
