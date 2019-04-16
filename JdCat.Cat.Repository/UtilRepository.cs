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
    public class UtilRepository : BaseRepository<Business>, IUtilRepository
    {
        public UtilRepository(CatDbContext context) : base(context)
        {
        }
        

    }
}
