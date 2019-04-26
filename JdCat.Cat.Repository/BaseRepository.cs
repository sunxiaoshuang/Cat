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
using System.Threading.Tasks;

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
        public int Delete<TEntity>(params TEntity[] entities) where TEntity : BaseEntity
        {
            Context.RemoveRange(entities);
            return Context.SaveChanges();
        }
        public async Task<int> DeleteAsync<TEntity>(params TEntity[] entities) where TEntity : BaseEntity
        {
            Context.RemoveRange(entities);
            return await Context.SaveChangesAsync();
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }
        public T Get(int id)
        {
            return Context.Set<T>().SingleOrDefault(a => a.ID == id);
        }
        public TEntity Get<TEntity>(int id) where TEntity : BaseEntity
        {
            return Context.Set<TEntity>().SingleOrDefault(a => a.ID == id);
        }
        public async Task<TEntity> GetAsync<TEntity>(int id) where TEntity : BaseEntity
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync(a => a.ID == id);
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<T>() : _context.Set<T>().Where(predicate);
        }
        public async Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : BaseEntity
        {
            return await Context.Set<TEntity>().ToListAsync();
        }
        public TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Context.Add(entity);
            Context.SaveChanges();
            return entity;
        }
        public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }
        public int Update<TEntity>(TEntity entity, IEnumerable<string> fieldNames = null, bool commit = true) where TEntity : BaseEntity
        {
            if (fieldNames == null || fieldNames.Count() == 0)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _context.Attach(entity);
                foreach (var field in fieldNames)
                {
                    _context.Entry(entity).Property(field).IsModified = true;
                }
            }
            if (commit) return Commit();
            return 0;
        }
        public async Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<string> fieldNames = null, bool commit = true) where TEntity : BaseEntity
        {
            if (fieldNames == null || fieldNames.Count() == 0)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _context.Attach(entity);
                foreach (var field in fieldNames)
                {
                    _context.Entry(entity).Property(field).IsModified = true;
                }
            }
            if (commit) return await CommitAsync();
            return 0;
        }
        public int Commit()
        {
            return _context.SaveChanges();
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
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
        /// <summary>
        /// 执行sql，返回单个值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            var conn = Context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            var result = command.ExecuteScalar();
            conn.Close();
            return result;

        }

        private static readonly ILog _log = LogManager.GetLogger(AppSetting.LogRepository.Name, typeof(BaseRepository<T>));
        /// <summary>
        /// 日志对象
        /// </summary>
        public ILog Log => _log;

    }
}
