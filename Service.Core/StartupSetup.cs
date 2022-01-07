using AutoMapper;
using Service.Core.Services.Implementation;
// using Service.Core.Services.Implementation.Elastic;
using Service.Core.Services.Interfaces;
using Service.Core.Services.MessagingEvent;
using Service.Infrastructure.DB.UnitOfWork;
using Service.Infrastructure.DB.Repositories.Sql;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.Messaging.Kafka;
using Service.Infrastructure.Messaging.Stan;
using Service.Infrastructure.Middleware;
using Service.Infrastructure.Models;
using Service.Infrastructure.DB.Context;
using Service.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Service.Core
{
    public static class StartupSetup
    {
        public static void AddCosmosDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CosmosDatabaseSettings>(opt => configuration.GetSection(nameof(CosmosDatabaseSettings)).Bind(opt));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<CosmosDatabaseSettings>>().Value);

            services.AddScoped<CosmosDBContext>();
            services.AddScoped<IUnitOfWork, CosmosUnitOfWork>();
        }

        public static void AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDatabaseSettings>(opt => configuration.GetSection(nameof(MongoDatabaseSettings)).Bind(opt));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value);

            services.AddSingleton<MongoDBContext>();
            services.AddSingleton<IUnitOfWork, MongoUnitOfWork>();
        }

        public static void AddSqlDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            SqlDatabaseSettings _settings = new SqlDatabaseSettings();
            configuration.GetSection(nameof(SqlDatabaseSettings)).Bind(_settings);

            services.AddDbContextPool<SqlDBContext>(x => {
                // x.UseSqlServer(
                x.UseNpgsql(
                                _settings.ServerUrl,
                                y => {
                                    y.CommandTimeout(600);
                                    // y.EnableRetryOnFailure(maxRetryCount: 100, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                                    y.EnableRetryOnFailure(maxRetryCount: 100, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
                                }
                                );
                x.EnableDetailedErrors();
                x.EnableSensitiveDataLogging();
            });

            services.AddScoped<IPropertyRepository, PropertyRepository>();

            services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
        }

        public static void AddElasticDbContext(this IServiceCollection services)
        {
            services.AddSingleton<ElasticDBContext>();
            services.AddSingleton<IUnitOfWork, ElasticUnitOfWork>();
        }

        public static void AddService(this IServiceCollection services, Type type)
        {
            services.AddAutoMapper(typeof(StartupSetup));

            //services.AddTransient<IStorage, LocalStorage>();
            services.AddTransient<IStorage, AzureStorage>();

            services.AddTransient<IPropertyService, PropertyService>();
            services.AddTransient<IRoomService, RoomService>();
        }

        public static void AddMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
        }

        public static void AddMessagingService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<IMessageSenderService, KafkaMessageSenderService>();
            //services.AddHostedService<KafkaMessageReceiverService>();

            services.AddTransient<IMessageSenderService, StanMessageSenderService>();
            services.AddHostedService<StanMessageReceiverService>();

            var topics = configuration.GetValue<string>("Messaging:Topics:Ping");

            //assign subscription by topic
            MessagingEvent messagingEvent = new MessagingEvent();
            messagingEvent.subscriptions = new Dictionary<string, Type> {
                { topics, typeof(TestEvent) }
            };

            services.AddSingleton(messagingEvent);
        }
    }
}
