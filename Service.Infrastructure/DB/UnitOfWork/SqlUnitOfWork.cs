using Microsoft.EntityFrameworkCore;
using Service.Infrastructure.DB.Context;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Repositories.Sql;
using System;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.UnitOfWork
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private DbContext dbContext;

        public IPropertyRepository PropertyRepository { get; }

        public SqlUnitOfWork(
            SqlDBContext context,
            IPropertyRepository propRepo
            )
        {
            dbContext = context;

            PropertyRepository = propRepo;
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
