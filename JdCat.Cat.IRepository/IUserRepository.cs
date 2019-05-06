using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// 微信端取得商户对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Business WxGetBusiness(int id);
        /// <summary>
        /// 根据openId获取用户
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        User Get(string openId);
        /// <summary>
        /// 授权用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User GrantInfo(User user);
        /// <summary>
        /// 授权手机号
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool GrantPhone(int id, string phone);
        /// <summary>
        /// 根据用户id获取地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Address> GetAddresses(int id);
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DelAddress(int id);
        /// <summary>
        /// 根据id获取地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Address GetAddress(int id);
        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool UpdateAddress(Address address);
        /// <summary>
        /// 根据用户获取购物车列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        IEnumerable<ShoppingCart> GetCarts(int businessId, int userId);
        /// <summary>
        /// 创建购物车
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        ShoppingCart CreateCart(ShoppingCart cart);
        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        ShoppingCart UpdateCart(ShoppingCart cart);
        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        void DeleteCart(ShoppingCart cart);
        /// <summary>
        /// 情况购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        bool ClearCart(int userId, int businessId);
        /// <summary>
        /// 更新购物车数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        bool UpdateCartQuantity(int id, int quantity);
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Order CreateOrder(Order order);
        /// <summary>
        /// 获取用户优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SaleCouponUser> GetUserCoupon(int businessId, int id);
        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<SaleCouponUser> ReceiveCoupons(User user, IEnumerable<int> ids);
        /// <summary>
        /// 获取商家用户
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
        IEnumerable<User> GetUsers(Business business);
        /// <summary>
        /// 获取用户评论（一年之内的）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<object> GetUserComments(int user);



    }
}
