using Service.Infrastructure.DB.Context;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Repositories.Mongo;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.UnitOfWork
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        public IPropertyRepository PropertyRepository { get; }

        public MongoUnitOfWork(MongoDBContext context)
        {
            PropertyRepository = new PropertyRepository(context);
        }

        public void Save()
        {
            //throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            return null;
        }
    }
}
