using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Service.Infrastructure.DB.Context
{
    public class CosmosDBContext : DbContext
    {
        #region Configuration
        CosmosDatabaseSettings _settings;

        public CosmosDBContext(IDatabaseSettings settings)
        {
            _settings = (CosmosDatabaseSettings)settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseCosmos(
                    _settings.ServerUrl,
                    _settings.ServerSecret, 
                    databaseName: _settings.DatabaseName);
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                .ToContainer("Properties")
                .OwnsMany(p => p.Rooms);
        }
    }
}
