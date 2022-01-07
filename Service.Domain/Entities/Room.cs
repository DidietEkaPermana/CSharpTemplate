using Service.Domain.Base;
using System;

namespace Service.Domain.Entities
{
    public class Room : BaseModel
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }
        public string RoomType { get; set; }
        public string Bed { get; set; }
        public ushort Guests { get; set; }
        public string Facilities { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int Available { get; set; }

        // [Column("Image", TypeName = "string[]")]
        // public string[] Image { get; set; }
        public string Image { get; set; }
    }
}
