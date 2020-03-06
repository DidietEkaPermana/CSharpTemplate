using Service.Infrastructure.DB.Context;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Repositories.Elastic;
using System.Threading.Tasks;

namespace Service.Infrastructure.DB.UnitOfWork
{
    public class ElasticUnitOfWork : IUnitOfWork
    {
        public IPropertyRepository PropertyRepository { get;  }

        public ElasticUnitOfWork(ElasticDBContext context)
        {
            PropertyRepository = new PropertyRepository(context);
        }

        public void Save()
        {
            //throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
