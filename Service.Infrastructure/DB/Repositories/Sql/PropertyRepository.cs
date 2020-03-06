using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.DB.Repositories.Sql
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(DbContext dbContext) : base(dbContext) { }
    }
}
