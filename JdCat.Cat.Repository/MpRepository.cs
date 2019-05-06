using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using JdCat.Cat.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Repository
{
    public class MpRepository : BaseRepository<Business>, IMpRepository
    {
        public MpRepository(CatDbContext context) : base(context)
        {
        }

        public async Task<List<Business>> GetBusinessForOpenIdAsync(string openid)
        {
            var ids = await Context.WxListenUsers.Where(a => a.openid == openid).Select(a => a.BusinessId).ToListAsync();
            if (!ids.Any()) return null;
            return await Context.Businesses.Where(a => ids.Contains(a.ID)).ToListAsync();
        }

    }
}
