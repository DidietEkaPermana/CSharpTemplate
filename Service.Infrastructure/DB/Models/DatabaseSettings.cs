using Service.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Infrastructure.DB.Models
{
    public class CosmosDatabaseSettings : IDatabaseSettings
    {
        public string ServerUrl { get; set; }
        public string ServerSecret { get; set; }
        public string DatabaseName { get; set; }
    }

    public class MongoDatabaseSettings : CosmosDatabaseSettings
    {

    }

    public class SqlDatabaseSettings : CosmosDatabaseSettings
    {

    }
}
