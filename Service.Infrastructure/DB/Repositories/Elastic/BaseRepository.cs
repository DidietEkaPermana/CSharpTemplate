using Service.Domain.Base;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Context;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.Repositories.Elastic
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        private ElasticDBContext _dbContext;
        public ElasticClient Collection { get; private set; }

        public BaseRepository(ElasticDBContext dbContext)
        {
            _dbContext = dbContext;
            Collection = _dbContext.DbSet<T>();
        }

        public T Add(T entity)
        {
            Collection.IndexDocument(entity);

            return entity;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Collection.IndexDocumentAsync(entity, cancellationToken);

            return entity;
        }

        public void AddRange(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public void AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public long Count(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            Collection.Delete<T>(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Edit(T entity)
        {
            UpdateResponse<T> updateResponse = Collection.Update<T>(
                DocumentPath<T>.Id(entity.Id), 
                p => p
                .DocAsUpsert(true)
                .Doc(entity)
                );
        }

        public T GetById(object id)
        {
            return Collection.Search<T>(
                s => s
                .Query(
                    q => q
                    .Match(
                        m => m
                        .Field(f => f.Id)
                        .Query(id.ToString())
                        )
                    )
                ).Documents.FirstOrDefault();
        }

        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return (await Collection.SearchAsync<T>(
                s => s
                .Query(
                    q => q
                    .Match(
                        m => m
                        .Field(f => f.Id)
                        .Query(id.ToString())
                    )
                )
            )).Documents.FirstOrDefault();
        }

        public List<T> GetByPage(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            //Func<QueryContainerDescriptor<T>, QueryContainer> _where
            //return Collection.Search<T>(
            //    s => s
            //    .From((pageIndex - 1) * pageSize)
            //    .Take(pageSize)
            //    .Query(where)
            //    ).Documents.ToList();
            throw new NotImplementedException();
        }

        public List<T> GetByPage(int pageSize, int pageIndex)
        {
            return Collection.Search<T>(
                s => s
                .From((pageIndex - 1) * pageSize)
                .Take(pageSize)
                ).Documents.ToList();
        }

        public Task<List<T>> GetByPageAsync(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            //Func<QueryContainerDescriptor<T>, QueryContainer> _where
            //return (await Collection.SearchAsync<T>(
            //    s => s
            //    .From((pageIndex - 1) * pageSize)
            //    .Size(pageSize)
            //    .Query(where)
            //    )).Documents.ToList();
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetByPageAsync(int pageSize, int pageIndex)
        {
            return (await Collection.SearchAsync<T>(
                s => s
                .From((pageIndex - 1) * pageSize)
                .Size(pageSize)
                )).Documents.ToList();
        }

        public List<T> GetMany(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public T GetSingle(object where, string include = null)
        {
            return GetSingle(where);
        }

        public T GetSingle(object where)
        {
            var _query = (Func<QueryContainerDescriptor<T>, QueryContainer>)where;
            var result = Collection.Search<T>(
                s => s
                .Query(_query)
                ).Documents.SingleOrDefault();

            return result;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            return (await Collection.SearchAsync<T>(
                s => s
                .Query(q => q.Term(where, true))
                )).Documents.SingleOrDefault();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, string include = null, CancellationToken cancellationToken = default)
        {
            return (await Collection.SearchAsync<T>(
                s => s
                .Query(q => q.Term(where, true))
                )).Documents.SingleOrDefault();
            //throw new NotImplementedException();
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
