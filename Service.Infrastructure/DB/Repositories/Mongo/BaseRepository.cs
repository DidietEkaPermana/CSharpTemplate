using Service.Domain.Base;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.Models;
using Service.Infrastructure.DB.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.Repositories.Mongo
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        private MongoDBContext _dbContext;
        public IMongoCollection<T> Collection { get; private set; }

        public BaseRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
            Collection = _dbContext.DbSet<T>();
        }

        public T Add(T entity)
        {
            Collection.InsertOne(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public void AddRange(List<T> entities)
        {
            Collection.InsertMany(entities);
        }

        public async void AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            await Collection.InsertManyAsync(entities);
        }

        public long Count(Expression<Func<T, bool>> where)
        {
            return Collection.CountDocuments(where);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await Collection.CountDocumentsAsync(where, cancellationToken: cancellationToken);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            Collection.DeleteMany(where);
        }

        public void Delete(T entity)
        {
            var filterId = Builders<T>.Filter.Eq("_id", entity.Id);
            Collection.DeleteOne(filterId);
        }

        public void DeleteRange(params T[] entities)
        {
            foreach (T entity in entities)
            {
                var filterId = Builders<T>.Filter.Eq("_id", entity.Id);
                Collection.DeleteOne(filterId);
            }
        }

        public void Edit(T entity)
        {
            var filterId = Builders<T>.Filter.Eq("_id", entity.Id);
            Collection.FindOneAndReplace(filterId, entity);
        }

        public T GetById(object id)
        {
            var filterId = Builders<T>.Filter.Eq("_id", id);
            var model = Collection.Find(filterId).FirstOrDefault();
            return model;
        }

        public async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var filterId = Builders<T>.Filter.Eq("_id", id);
            var model = await Collection.FindAsync(filterId, cancellationToken:cancellationToken);
            return await model.FirstOrDefaultAsync(cancellationToken);
        }

        public List<T> GetByPage(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return Collection.Find(where).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
        }

        public List<T> GetByPage(int pageSize, int pageIndex)
        {
            return Collection.Find(p => true).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
        }

        public async Task<List<T>> GetByPageAsync(Expression<Func<T, bool>> where, int pageSize, int pageIndex)
        {
            return await Collection.Find(where).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
        }

        public async Task<List<T>> GetByPageAsync(int pageSize, int pageIndex)
        {
            return await Collection.Find(p => true).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
        }

        public List<T> GetMany(Expression<Func<T, bool>> where)
        {
            return Collection.Find(where).ToList();
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await Collection.Find(where).ToListAsync();
        }

        public T GetSingle(object where)
        {
            var _where = (Expression<Func<T, bool>>)where;
            return Collection.Find(_where).FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await Collection.Find(where).FirstOrDefaultAsync();
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return (Collection.Find(predicate).CountDocuments() > 0);
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return (await Collection.Find(predicate).CountDocumentsAsync() > 0);
        }

        public T GetSingle(object where, string include = null)
        {
            return GetSingle(where);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, string include = null, CancellationToken cancellationToken = default)
        {
            return await Collection.Find(where).FirstOrDefaultAsync();
        }
    }
}
