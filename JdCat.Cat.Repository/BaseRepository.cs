using System;
using System.Linq;
using System.Linq.Expressions;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using Microsoft.EntityFrameworkCore;

namespace JdCat.Cat.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
    {
        protected readonly CatDbContext _context;
        public BaseRepository(CatDbContext context)
        {
            _context = context;
        }
        public int Count()
        {
            return _context.Set<T>().Count();
        }
        public void Delete(T entity, bool commit = true)
        {
            _context.Set<T>().Remove(entity);
            if (commit) Commit();
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>() : _context.Set<T>().Where(predicate);
        }
        public void Add(T entity, bool commit = true)
        {
            if (entity.CreateTime == null)
            {
                entity.CreateTime = DateTime.Now;
            }
            _context.Set<T>().Add(entity);
            if (commit) Commit();
        }
        public void Update(T entity, bool commit = true)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            if (commit) Commit();
        }
        public int Commit()
        {
            return _context.SaveChanges();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }
    }
}
