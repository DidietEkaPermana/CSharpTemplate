using Elasticsearch.Net;
using Service.Domain.Base;
using Microsoft.Extensions.Configuration;
using Nest;
using System;

namespace Service.Infrastructure.DB.Context
{
    public class ElasticDBContext
    {
        ConnectionSettings connectionSettings;

        public ElasticDBContext(IConfiguration settings)
        {
            //var uris = new[]
            //{
            //    new Uri(settings["ElasticConfiguration:Uri"])
            //};

            //var connectionPool = new SniffingConnectionPool(uris);
            //connectionSettings = new ConnectionSettings(connectionPool);

            connectionSettings = new ConnectionSettings(new Uri(settings["ElasticConfiguration:Uri"]));
            connectionSettings.ThrowExceptions(true);
        }

        public ElasticClient DbSet<T>() where T : BaseModel
        {
            string indexName = typeof(T).FullName.ToLower();
            //connectionSettings.DefaultMappingFor<T>(i => i.IndexName(indexName).IdProperty(p => p.Id));
            connectionSettings.DefaultIndex(indexName);
            return new ElasticClient(connectionSettings);
        }
    }
}
