using Service.Domain.Base;
using Service.Infrastructure.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Service.Infrastructure.DB.Context
{
    public class MongoDBContext
    {
        private IMongoDatabase mongoDatabase;

        public MongoDBContext(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ServerUrl);
            mongoDatabase = client.GetDatabase(settings.DatabaseName);

            BsonClassMap.RegisterClassMap<BaseModel>(parameterMap =>
            {
                parameterMap.AutoMap();
                parameterMap.MapIdMember(parameter => parameter.Id);
                parameterMap.GetMemberMap(parameter => parameter.Id).SetDefaultValue(Guid.NewGuid());
            });
        }

        public IMongoCollection<T> DbSet<T>() where T : BaseModel
        {
            return mongoDatabase.GetCollection<T>(typeof(T).Name);
        }
    }
}
