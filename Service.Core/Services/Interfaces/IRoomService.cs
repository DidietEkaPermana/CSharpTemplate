using Service.Core.DTOs;
using System.Collections.Generic;

namespace Service.Core.Services.Interfaces
{
    public interface IRoomService
    {
        List<RoomOutput> Get(string propertyId);
        RoomOutput Get(string propertyId, string roomId);
        RoomOutput Add(string propertyId, RoomInput newRoom);
        void Edit(string propertyId, string roomId, RoomInput newRoom);
        void Delete(string propertyId, string roomId);
    }
}