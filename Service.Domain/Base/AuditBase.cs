using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Domain.Base
{
    public class AuditBase
    {
        public string CreatedId { get; set; }
        public string CreatedName { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdatedId { get; set; }
        public string UpdatedName { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
