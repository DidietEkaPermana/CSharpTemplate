using Service.Domain.Base;
using System.Collections.Generic;

namespace Service.Domain.Entities
{
    public class Property : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PropertyType Type { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Facilities { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
