using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Context;

namespace Service.Infrastructure.DB.Repositories.Mongo
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(MongoDBContext dbContext) : base(dbContext) { }
    }
}
