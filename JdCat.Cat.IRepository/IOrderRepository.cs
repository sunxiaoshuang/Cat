﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Common;
using JdCat.Cat.Common.Dianwoda;
using JdCat.Cat.Common.Models;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.AspNetCore.Hosting;

namespace JdCat.Cat.IRepository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        //Order Get(int id);
        /// <summary>
        /// 创建新订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        JsonData CreateOrder(Order order);
        /// <summary>
        /// 预估运费
        /// </summary>
        /// <returns></returns>
        Task<JsonData> EstimateFreight(int businessId, double lat, double lng, string address);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="status">订单状态</param>
        /// <param name="query">分页参数</param>
        /// <param name="code">订单编号</param>
        /// <param name="phone">用户手机号</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        IEnumerable<Order> GetOrder(Business business, OrderStatus? status, PagingQuery query, string code, string phone, int? userId = null, Expression<Func<Order, bool>> expression = null, DateTime? createTime = null);
        /// <summary>
        ///  客户端获取订单，有效订单
        /// </summary>
        /// <param name="businessId">商户id</param>
        /// <param name="createTime">下单日期</param>
        /// <param name="query">查询参数</param>
        /// <returns></returns>
        IEnumerable<Order> GetOrder(int businessId, DateTime createTime, PagingQuery query);
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="business">商户对象</param>
        /// <param name="status">订单状态</param>
        /// <param name="query">分页参数</param>
        /// <param name="code">订单编号</param>
        /// <param name="phone">用户手机号</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        IEnumerable<Order> GetOrders(Business business, OrderStatus? status, PagingQuery query, string code, string phone, int? userId = null, Expression<Func<Order, bool>> expression = null, DateTime? createTime = null);
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order GetOrderForDetail(int id);
        /// <summary>
        /// 载入套餐商品
        /// </summary>
        /// <param name="products"></param>
        void QuarySetMealProduct(IEnumerable<OrderProduct> products);
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
        JsonData Reject(int id, string reason, X509Certificate2 cert);
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        JsonData Cancel(int id, string reason, X509Certificate2 cert);
        /// <summary>
        /// 自己配送订单
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        bool SendOrderSelf(int id, Order order = null);
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
        /// 一城飞客回调更新
        /// </summary>
        /// <param name="dada"></param>
        void UpdateOrderStatus(YcfkCallback ycfk);
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
        /// 获取打印机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FeyinDevice GetPrinter(int id);
        /// <summary>
        /// 获取订单，包含产品集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order GetOrderIncludeProduct(int id);
        /// <summary>
        /// 获取订单，包含产品集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order GetOrderOnlyProduct(int id);
        /// <summary>
        /// 订单支付成功
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Order> PaySuccessAsync(WxPaySuccess ret);
        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order">订单</param>
        /// <param name="business">商户</param>
        /// <param name="device">打印机</param>
        Task<string> Print(Order order, FeyinDevice device = null, Business business = null);
        /// <summary>
        /// 根据订单编码获取订单
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Order GetOrderByCode(string code);
        /// <summary>
        /// 根据订单编号获取订单
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Order> GetOrderByCodeAsync(string code);
        /// <summary>
        /// 根据订单编号获取订单id
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        int GetOrderIdByCode(string orderCode);

        /// <summary>
        /// 根据商户id获取点我达商户
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        DWDStore GetDwdShop(int businessId);
        /// <summary>
        /// 根据点我达的回调，更新订单的状态
        /// </summary>
        /// <param name="callback">点我达回调参数</param>
        /// <param name="cost">订单配送费用</param>
        Order UpdateOrderByDwd(DWD_Callback callback, double cost);
        /// <summary>
        /// 自动接单
        /// </summary>
        /// <param name="order"></param>
        Task AutoReceipt(Order order);
        /// <summary>
        /// 配送订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<JsonData> Invoice(Order order);
        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        JsonData ApplyRefund(int id, string reason);
        /// <summary>
        /// 获取订单骑手位置
        /// </summary>
        /// <param name="id"></param>
        YcfkLocation GetOrderLocation(int id);
        /// <summary>
        /// 订单评论
        /// </summary>
        /// <returns></returns>
        JsonData Comment(OrderComment comment);
        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="deliveryLevel">配送评论级别</param>
        /// <param name="orderLevel">订单评论级别</param>
        /// <param name="start">查询开始日期</param>
        /// <param name="end">查询结束日期</param>
        /// <param name="paging">查询参数</param>
        /// <param name="businessIds">需要查询的商户id集合</param>
        /// <returns></returns>
        List<OrderComment> GetComments(CommentLevel deliveryLevel, CommentLevel orderLevel, DateTime start, DateTime end, PagingQuery paging, params int[] businessIds);
        /// <summary>
        /// 切换评论的显示状态
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="visible"></param>
        void ChangeCommentVisible(int commentId, bool visible);
        /// <summary>
        /// 重新计算商户的评分
        /// </summary>
        /// <param name="id"></param>
        void ReloadCommentScore(int id);

        /// <summary>
        /// 新增订单通知
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> AddOrderNotifyAsync(Order order);

        /// <summary>
        /// 根据商户的订单各种状态，查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mode"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrderByStatus(int id, DeliveryMode mode = DeliveryMode.Third, OrderStatus status = OrderStatus.Unfinish, DateTime? startTime = null, DateTime? endTime = null);




        //Task<List<Order>> GetOrdersIncludeProductAsync(DateTime? endDate = null);
        //Task<List<Order>> GetOrdersNotActivityAsync();







    }
}
