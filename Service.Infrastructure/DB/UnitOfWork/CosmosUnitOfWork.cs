using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Context;
using Service.Infrastructure.DB.Repositories.Cosmos;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service.Infrastructure.DB.UnitOfWork
{
    public class CosmosUnitOfWork : IUnitOfWork
    {
        private DbContext dbContext;

        public IPropertyRepository PropertyRepository { get; }

        public CosmosUnitOfWork(CosmosDBContext context)
        {
            dbContext = context;
            PropertyRepository = new PropertyRepository(dbContext);
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
