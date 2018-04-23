using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace JdCat.Cat.IRepository
{
    public interface IBaseRepository<T> where T : class, new()
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        T Get(Expression<Func<T, bool>> predicate);
        void Add(T entity, bool commit = true);
        void Delete(T entity, bool commit = true);
        void Update(T entity, bool commit = true);
        int Count();
        int Commit();
    }
}
