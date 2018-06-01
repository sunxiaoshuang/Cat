using System;
using System.Collections.Generic;
using System.Text;
using JdCat.Cat.Model.Data;

namespace JdCat.Cat.IRepository
{
    public interface ISessionDataRepository : IBaseRepository<SessionData>
    {
        SessionData SetSession(SessionData entity);
    }
}
