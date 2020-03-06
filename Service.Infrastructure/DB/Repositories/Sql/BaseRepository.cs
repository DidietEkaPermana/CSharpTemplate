using Service.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.Repositories.Sql
{
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        private readonly DbSet<T> dbSet;
        public DbContext Context { get; private set; }

        public BaseRepository(DbContext dbContext)
        {
            Context = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public T Add(T entity)
        {
            return dbSet.Add(entity).Entity;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            return (await dbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        public void AddRange(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public async void AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public long Count(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).Count();
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await dbSet.Where(where).CountAsync(cancellationToken);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            dbSet.RemoveRange(dbSet.Where(where).ToList());
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        public T GetById(object id)
        {
            return dbSet.Find(id);
        }

        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await dbSet.FindAsync(id, cancellationToken);
        }

        public List<T> GetByPage(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return dbSet.Where(where).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<T> GetByPage(int pageSize, int pageIndex)
        {
            return dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<List<T>> GetByPageAsync(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return await dbSet.Where(where).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<T>> GetByPageAsync(int pageSize, int pageIndex)
        {
            return await dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public List<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }

        public T GetSingle(object where)
        {
            var _where = (Expression<Func<T, bool>>)where;
            return dbSet.SingleOrDefault(_where);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await dbSet.SingleOrDefaultAsync(where);
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return (dbSet.Where(predicate).Count() > 0);
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return ((await dbSet.Where(predicate).CountAsync()) > 0);
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }

        public T GetSingle(object where, string include = null)
        {
            var _where = (Expression<Func<T, bool>>)where;
            return dbSet.Include(include).SingleOrDefault(_where);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, string include = null, CancellationToken cancellationToken = default)
        {
            return await dbSet.Include(include).SingleOrDefaultAsync(where);
        }

        public T GetSingle(Func<T, T> field, object value)
        {
            throw new NotImplementedException();
        }
    }
}
