using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Domain.Base
{
    public class BaseModel : AuditBase
    {
        public Guid Id { get; set; }
    }
}
