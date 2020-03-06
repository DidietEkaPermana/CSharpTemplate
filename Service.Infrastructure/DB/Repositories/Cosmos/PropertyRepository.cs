using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Service.Infrastructure.DB.Repositories.Cosmos
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(DbContext dbContext) : base(dbContext) { }
    }
}
