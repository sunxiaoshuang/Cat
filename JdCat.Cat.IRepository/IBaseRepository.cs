using JdCat.Cat.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JdCat.Cat.IRepository
{
    public interface IBaseRepository<T> where T : class, new()
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// 获取类型所有对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : BaseEntity;
        T Get(Expression<Func<T, bool>> predicate);
        T Get(int id);
        TEntity Get<TEntity>(int id) where TEntity : BaseEntity;
        Task<TEntity> GetAsync<TEntity>(int id) where TEntity : BaseEntity;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity;
        Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
        Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : BaseEntity;
        int Delete<TEntity>(params TEntity[] entities) where TEntity : BaseEntity;
        Task<int> DeleteAsync<TEntity>(params TEntity[] entities) where TEntity : BaseEntity;
        int Update<TEntity>(TEntity entity, IEnumerable<string> fieldNames = null, bool commit = true) where TEntity : BaseEntity;
        Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<string> fieldNames = null, bool commit = true) where TEntity : BaseEntity;
        int Count();
        int Commit();
        Task<int> CommitAsync();
        bool Exists(Expression<Func<T, bool>> predicate);
    }
}
