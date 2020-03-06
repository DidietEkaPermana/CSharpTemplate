using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        void AddRange(List<T> entities);
        void AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
        long Count(Expression<Func<T, bool>> where);
        Task<long> CountAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default);
        void Delete(Expression<Func<T, bool>> where);
        void Delete(T entity);
        void DeleteRange(params T[] entities);
        void Edit(T entity);
        T GetById(object id);
        Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        List<T> GetByPage(Expression<Func<T, bool>> where, int pageSize, int pageIndex);
        List<T> GetByPage(int pageSize, int pageIndex);
        Task<List<T>> GetByPageAsync(Expression<Func<T, bool>> where, int pageSize, int pageIndex);
        Task<List<T>> GetByPageAsync(int pageSize, int pageIndex);
        List<T> GetMany(Expression<Func<T, bool>> where);
        Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where);
        T GetSingle(object where, string include = null);
        T GetSingle(object where);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> where, string include = null, CancellationToken cancellationToken = default);
        bool IsExist(Expression<Func<T, bool>> predicate);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}