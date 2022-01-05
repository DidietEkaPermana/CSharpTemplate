using System;

namespace Service.Infrastructure.Models
{
    public class FileProperty
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string ModifiedOn { get; set; }
        public string Uri { get; set; }
    }
}