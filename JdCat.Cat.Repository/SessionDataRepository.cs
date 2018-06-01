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
    public class SessionDataRepository : BaseRepository<SessionData>, ISessionDataRepository
    {
        public SessionDataRepository(CatDbContext context) : base(context)
        {
        }

        public SessionData SetSession(SessionData entity)
        {
            Context.Database.ExecuteSqlCommand($"delete dbo.SessionData where UserId={entity.UserId}");
            Context.SessionDatas.Add(entity);
            Commit();
            return entity;
        }
    }
}
