using Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.DB.Context
{
    public class SqlDBContext : DbContext
    {
        public SqlDBContext(DbContextOptions<SqlDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .Property(i => i.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Room>()
                .Property(i => i.Price)
                .HasColumnType("money");
        }
    }
}
