using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using JdCat.Cat.IRepository;
using JdCat.Cat.Model;
using Microsoft.EntityFrameworkCore;
using JdCat.Cat.Common;
using log4net;

namespace JdCat.Cat.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, new()
    {
        private readonly CatDbContext _context;
        public CatDbContext Context => _context;
        public BaseRepository(CatDbContext context)
        {
            _context = context;
        }
        public int Count()
        {
            return _context.Set<T>().Count();
        }
        public int Delete(T entity, bool commit = true)
        {
            _context.Set<T>().Remove(entity);
            if (commit) return Commit();
            return 0;
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }
        public T Get(int id)
        {
            return Context.Set<T>().SingleOrDefault(a => a.ID == id);
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>() : _context.Set<T>().Where(predicate);
        }
        public int Add(T entity, bool commit = true)
        {
            if (entity.CreateTime == null)
            {
                entity.CreateTime = DateTime.Now;
            }
            _context.Set<T>().Add(entity);
            if (commit) return Commit();
            return 0;
        }
        public int Update(T entity, IEnumerable<string> fieldNames = null, bool commit = true)
        {
            if (fieldNames == null || fieldNames.Count() == 0)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _context.Set<T>().Attach(entity);
                foreach (var field in fieldNames)
                {
                    _context.Entry(entity).Property(field).IsModified = true;
                }
            }
            if (commit) return Commit();
            return 0;
        }
        public int Commit()
        {
            return _context.SaveChanges();
        }
        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate) != null;
        }
        public DataTable ExecuteReader(string search)
        {
            var conn = Context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = search;
            command.CommandType = CommandType.Text;
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            conn.Close();
            return dt;
        }
        public List<TModel> ExecuteReader<TModel>(string search) where TModel : new()
        {
            var dt = ExecuteReader(search);
            return dt.GetList<TModel>();
        }

        private static readonly ILog _log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(BaseRepository<T>));
        /// <summary>
        /// 日志对象
        /// </summary>
        public ILog Log => _log;

    }
}
