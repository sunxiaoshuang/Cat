using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;

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

        public bool GrantInfo(User user)
        {
            var entity = new User { ID = user.ID};
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
            return Context.SaveChanges() > 0;
        }

        public bool GrantPhone(string phone)
        {
            throw new NotImplementedException();
        }

    }
}
