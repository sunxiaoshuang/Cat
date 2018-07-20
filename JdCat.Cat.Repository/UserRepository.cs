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
        public IEnumerable<ShoppingCart> GetCarts(int userId)
        {
            return Context.ShoppingCarts.Where(a => a.UserId == userId).ToList();
            //return Context.ShoppingCarts.FromSql($@"
            //        select a.* from dbo.ShoppingCart a
            //            inner join dbo.Product b on a.ProductId=b.Id and b.Status={(int)ProductStatus.Sale}
            //            where UserId={userId}").ToList();
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
            entity.Quantity = cart.Quantity;
            Context.SaveChanges();
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
            return Context.Database.ExecuteSqlCommand("delete dbo.ShoppingCart where userid={0} and businessid={1}", userId, businessId) > 0;
            
        }
        public Order CreateOrder(Order order)
        {
            Context.Orders.Add(order);
            if(Context.SaveChanges() == 0)
            {
                throw new Exception("订单创建失败");
            }
            ClearCart(order.UserId.Value, order.BusinessId.Value);
            return order;
        }
    }
}
