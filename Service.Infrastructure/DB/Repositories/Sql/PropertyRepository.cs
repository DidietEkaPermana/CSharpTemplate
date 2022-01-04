using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using Service.Infrastructure.DB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.DB.Repositories.Sql
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ILogger<PropertyRepository> logger, SqlDBContext dbContext) : base(dbContext) { }
    }
}
