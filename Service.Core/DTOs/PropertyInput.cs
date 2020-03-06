using Service.Domain.Entities;

namespace Service.Core.DTOs
{
    public class PropertyInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PropertyType Type { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Facilities { get; set; }
    }
}
