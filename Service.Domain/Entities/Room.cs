using Service.Domain.Base;
using System;

namespace Service.Domain.Entities
{
    public class Room : BaseModel
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public string Bed { get; set; }
        public ushort Guests { get; set; }
        public string Facilities { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public ushort Capacity { get; set; }
    }
}
