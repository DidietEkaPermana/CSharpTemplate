using Service.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.Repositories.Cosmos
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbSet<T> dbSet;
        private DbContext Context { get; set; }

        public BaseRepository(DbContext dbContext)
        {
            Context = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public T Add(T entity)
        {
            T result = dbSet.Add(entity).Entity;
            return result;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entry = await dbSet.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public void AddRange(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public async void AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Edit(T entity)
        {
            dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            List<T> objects = dbSet.Where<T>(where).ToList();
            dbSet.RemoveRange(objects);
        }

        public T GetById(object id)
        {
            return dbSet.Find(id);
        }

        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbSet.FindAsync(id, cancellationToken);
        }

        public T GetSingle(object where)
        {
            Expression<Func<T, bool>> _where = (Expression<Func<T, bool>>)where;
            return dbSet.FirstOrDefault<T>(_where);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await dbSet.FirstOrDefaultAsync<T>(where, cancellationToken);
        }

        public List<T> GetByPage(int pageSize, int pageIndex)
        {
            return dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<List<T>> GetByPageAsync(int pageSize, int pageIndex)
        {
            return await dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public List<T> GetByPage(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return dbSet.Where(where).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<List<T>> GetByPageAsync(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return await dbSet.Where(where).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public List<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(predicate, cancellationToken);
        }

        public long Count(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).Count();
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await dbSet.Where(where).CountAsync(cancellationToken);
        }

        public T GetSingle(object where, string include = null)
        {
            return GetSingle(where);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, string include = null, CancellationToken cancellationToken = default)
        {
            return await dbSet.FirstOrDefaultAsync<T>(where);
        }
    }
}
