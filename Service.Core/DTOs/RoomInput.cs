using Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core.DTOs
{
    public class RoomInput
    {
        public string PropertyId { get; set; }
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
