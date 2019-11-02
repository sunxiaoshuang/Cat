using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using JdCat.Cat.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CatDbContext context) : base(context)
        {
        }

        public Business WxGetBusiness(int id)
        {
            return Context.Businesses.AsNoTracking().Single(a => a.ID == id);
            //var noValid = business.SaleFullReduces.Where(a => !a.IsActiveValid());
            //if(noValid.Count() > 0)
            //{
            //    foreach (var item in noValid)
            //    {
            //        business.SaleFullReduces.Remove(item);
            //    }
            //}
        }
        /// <summary>
        /// 根据openId获取用户
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public User Get(string openId)
        {
            return Context.Users.FirstOrDefault(a => a.OpenId == openId);
        }

        public User GrantInfo(User user)
        {
            var entity = new User { ID = user.ID };
            Context.Attach(entity);
            entity.Age = user.Age;
            entity.AvatarUrl = user.AvatarUrl;
            entity.City = user.City;
            entity.Country = user.Country;
            entity.Gender = user.Gender;
            entity.Language = user.Language;
            entity.NickName = user.NickName;
            entity.Province = user.Province;
            entity.Phone = user.Phone;
            entity.IsRegister = true;
            if (user.BusinessId > 0)
            {
                entity.BusinessId = user.BusinessId;
            }
            Context.SaveChanges();
            return entity;
        }

        public bool GrantPhone(int id, string phone)
        {
            var entity = new User { ID = id };
            Context.Attach(entity);
            entity.Phone = phone;
            entity.IsPhone = true;
            return Context.SaveChanges() > 0;
        }

        public IEnumerable<Address> GetAddresses(int id)
        {
            return Context.Addresses.Where(a => a.UserId == id);
        }
        public bool DelAddress(int id)
        {
            Context.Addresses.Remove(new Address { ID = id });
            return Context.SaveChanges() > 0;
        }
        public Address GetAddress(int id)
        {
            return Context.Addresses.Find(id);
        }
        public bool UpdateAddress(Address address)
        {
            var entity = new Address { ID = address.ID };
            Context.Attach(entity);
            entity.AreaName = address.AreaName;
            entity.CityName = address.CityName;
            entity.DetailInfo = address.DetailInfo;
            entity.Gender = address.Gender;
            entity.Lat = address.Lat;
            entity.Lng = address.Lng;
            entity.MapInfo = address.MapInfo;
            entity.Phone = address.Phone;
            entity.PostalCode = address.PostalCode;
            entity.ProvinceName = address.ProvinceName;
            entity.Receiver = address.Receiver;
            entity.ModifyTime = DateTime.Now;
            return Context.SaveChanges() > 0;
        }
        public IEnumerable<ShoppingCart> GetCarts(int businessId, int userId)
        {
            if (businessId == 0)
            {
                return Context.ShoppingCarts.Where(a => a.UserId == userId).ToList();
            }
            return Context.ShoppingCarts.Where(a => a.UserId == userId && a.BusinessId == businessId).ToList();
        }
        public async Task<List<ShoppingCart>> GetCartsAsync(int businessId, int userId)
        {
            var query = Context.ShoppingCarts.Where(a => a.UserId == userId);
            if (businessId > 0) query = query.Where(a => a.BusinessId == businessId);
            return await query.ToListAsync();
        }
        public ShoppingCart CreateCart(ShoppingCart cart)
        {
            Context.ShoppingCarts.Add(cart);
            Context.SaveChanges();
            return cart;
        }
        public ShoppingCart UpdateCart(ShoppingCart cart)
        {
            var entity = new ShoppingCart { ID = cart.ID };
            Context.Attach(entity);
            if (entity.Quantity != cart.Quantity)
            {
                entity.Quantity = cart.Quantity;
                Context.SaveChanges();
            }
            return cart;
        }
        public void DeleteCart(ShoppingCart cart)
        {
            Context.ShoppingCarts.Remove(cart);
            Context.SaveChanges();
        }
        public bool UpdateCartQuantity(int id, int quantity)
        {
            var cart = new ShoppingCart { ID = id };
            if (quantity <= 0)
            {
                Context.ShoppingCarts.Remove(cart);
            }
            else
            {
                Context.ShoppingCarts.Attach(cart);
                cart.Quantity = quantity;
            }
            return Context.SaveChanges() > 0;
        }
        public bool ClearCart(int userId, int businessId)
        {
            return Context.Database.ExecuteSqlCommand("delete from `ShoppingCart` where userid={0} and businessid={1}", userId, businessId) > 0;

        }
        public async Task<int> ClearCartAsync(int userId, int businessId)
        {
            var carts = await Context.ShoppingCarts
                .Where(a => a.UserId == userId && a.BusinessId == businessId)
                .Select(a => a.ID)
                .ToListAsync();
            if (carts.Count == 0) return 0;
            Context.RemoveRange(carts.Select(a => new ShoppingCart { ID = a }));
            return await Context.SaveChangesAsync();
        }
        public Order CreateOrder(Order order)
        {
            Context.Orders.Add(order);
            if (Context.SaveChanges() == 0)
            {
                throw new Exception("订单创建失败");
            }
            ClearCart(order.UserId.Value, order.BusinessId.Value);
            return order;
        }
        public List<SaleCouponUser> GetUserCoupon(int businessId, int id)
        {
            var now = DateTime.Now;

            var query = Context.SaleCouponUsers.Where(a => a.UserId == id);
            // 取得未使用或者半年内领取的优惠券
            var list = query.Where(a => a.CreateTime > now.AddDays(-180) || a.Status == CouponStatus.NotUse).ToList();
            //if (businessId == 0)
            //{
            //    list = Context.SaleCouponUsers.Include(a => a.Coupon).Where(a => a.UserId == id).ToList();
            //}
            //else
            //{
            //    list = Context.SaleCouponUsers.Include(a => a.Coupon).Where(a => a.UserId == id && a.Coupon.BusinessId == businessId).ToList();
            //}
            var coupons = Context.SaleCoupons.Where(a => a.BusinessId == businessId).ToList();
            var retCoupons = Context.SaleReturnCoupons.Where(a => a.BusinessId == businessId).ToList();
            var result = new List<SaleCouponUser>();
            list.ForEach(item =>
            {
                var coupon = coupons.FirstOrDefault(a => a.ID == item.CouponId);
                if (coupon != null)
                {
                    result.Add(item);
                    return;
                }
                var retCoupon = retCoupons.FirstOrDefault(a => a.ID == item.ReturnCouponId);
                if (retCoupon != null)
                {
                    result.Add(item);
                    return;
                }
            });
            return result;
        }
        public List<SaleCouponUser> ReceiveCoupons(User user, IEnumerable<int> ids)
        {
            var coupons = Context.SaleCoupons.Where(a => ids.Contains(a.ID));
            // 将有数量限制的优惠券筛选出来，领取后需要库存减一
            var canReceive = new List<SaleCoupon>();
            foreach (var item in coupons)
            {
                if (item.Quantity <= 0)
                {
                    canReceive.Add(item);
                    continue;
                }
                if (item.Stock > 0)
                {
                    canReceive.Add(item);
                }
            }

            var list = new List<SaleCouponUser>();
            foreach (var coupon in canReceive)
            {
                var item = new SaleCouponUser
                {
                    Name = coupon.Name,
                    Value = coupon.Value,
                    Status = CouponStatus.NotUse,
                    MinConsume = coupon.MinConsume,
                    StartDate = coupon.StartDate,
                    EndDate = coupon.EndDate,
                    ValidDay = coupon.ValidDay,
                    UserId = user.ID,
                    Code = ExecuteScalar("SELECT CONCAT(fn_right_padding(floor(rand()*10000000), 6), NEXT_VAL('SaleCouponNumbers'), fn_right_padding(floor(rand()*100000), 4))") + "",
                    CouponId = coupon.ID
                };
                if (coupon.Quantity > 0)
                {
                    coupon.Stock = coupon.Stock - 1;
                    if (coupon.Stock < 0) coupon.Stock = 0;
                    coupon.Received = coupon.Quantity - coupon.Stock;
                }
                else
                {
                    coupon.Received += 1;
                }
                list.Add(item);
                Context.Add(item);
            }

            Context.SaveChanges();
            return list;
        }

        public async Task ReceiveReturnCouponsAsync(IEnumerable<SaleCouponUser> coupons)
        {
            var ids = coupons.Select(a => a.ReturnCouponId).ToList();
            var entities = await Context.SaleReturnCoupons.Where(a => ids.Contains(a.ID)).ToListAsync();
            await Context.AddRangeAsync(coupons);
            entities.ForEach(entity => 
            {
                entity.Received++;
                if(entity.Stock > 0)
                {
                    entity.Stock--;
                }
            });
            await Context.SaveChangesAsync();
        }

        public IEnumerable<User> GetUsers(Business business)
        {
            return Context.Users.Where(user => user.BusinessId == business.ID && user.IsRegister).OrderByDescending(a => a.CreateTime);
        }

        public async Task<object> GetUserComments(int user)
        {
            var start = DateTime.Now.AddYears(-1);
            var end = DateTime.Now;
            var query = from comment in Context.OrderComments
                        join business in Context.Businesses on comment.BusinessId equals business.ID
                        where comment.CreateTime > start && comment.CreateTime < end && comment.UserId == user
                        select new { comment.CreateTime, comment.OrderScore, comment.DeliveryScore, comment.OrderId, comment.CommentContent, comment.CommentResult, comment.DeliveryResult, comment.ReplyContent, business.Name };
            return await query.ToListAsync();
        }
        //public bool SetPrimaryUser(Business business)
        //{
        //    var entity = new Business { ID = business.ID };
        //    Context.Attach(entity);
        //    entity.TemplateNotifyUser = business.TemplateNotifyUser;
        //    return Context.SaveChanges() > 0;
        //}
    }
}
