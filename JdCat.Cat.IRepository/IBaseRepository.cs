using JdCat.Cat.Model;
using Microsoft.EntityFrameworkCore;
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
        T Get(int id);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity;
        int Delete<TEntity>(params TEntity[] entities) where TEntity : BaseEntity;
        int Update<TEntity>(TEntity entity, IEnumerable<string> fieldNames = null, bool commit = true) where TEntity : BaseEntity;
        int Count();
        int Commit();
        bool Exists(Expression<Func<T, bool>> predicate);
    }
}
