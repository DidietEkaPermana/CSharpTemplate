using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Context;

namespace Service.Infrastructure.DB.Repositories.Elastic
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ElasticDBContext dbContext) : base(dbContext) { }
    }
}
