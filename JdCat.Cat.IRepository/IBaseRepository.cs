using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace JdCat.Cat.IRepository
{
    public interface IBusinessRepository<T> where T : class, new()
    {
        /// <returns></returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        T Get(Expression<Func<T, bool>> predicate);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int Add(T entity, bool commit = true);
        int Delete(T entity, bool commit = true);
        int Update(T entity, IEnumerable<string> fieldNames = null, bool commit = true);
        //void Update(T entity, Expression<Func<T, T>> fieldNames = null, bool commit = true);
        int Count();
        int Commit();
        bool Exists(Expression<Func<T, bool>> predicate);
    }
}
