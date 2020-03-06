using AutoMapper;
using Service.Core.DTOs;
using Service.Core.Services.Interfaces;
using Service.Domain.Entities;
using Service.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service.Core.Services.Implementation
{
    public class RoomService : IRoomService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<RoomOutput> Get(string propertyId)
        {
            try
            {
                Expression<Func<Property, bool>> where = p => p.Id == Guid.Parse(propertyId);

                Property property = _unitOfWork.PropertyRepository.GetSingle(where, "Rooms");
                
                if (property.Rooms != null)
                    return _mapper.Map<List<RoomOutput>>(property.Rooms);

                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public RoomOutput Get(string propertyId, string roomId)
        {
            try
            {
                Expression<Func<Property, bool>> where = p => p.Id == Guid.Parse(propertyId);

                Property property = _unitOfWork.PropertyRepository.GetSingle(where, "Rooms");

                if (property.Rooms != null)
                {
                    return _mapper.Map<RoomOutput>(((List<Room>)property.Rooms).Find(p => p.Id == Guid.Parse(roomId)));
                }

                throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public RoomOutput Add(string propertyId, RoomInput newRoom)
        {
            try
            {
                Room room = _mapper.Map<Room>(newRoom);
                room.Id = Guid.NewGuid();
                room.PropertyId = Guid.Parse(propertyId);
                room.UpdatedDate = room.CreateDate = DateTime.UtcNow;

                Expression<Func<Property, bool>> where = p => p.Id == Guid.Parse(propertyId);

                Property property = _unitOfWork.PropertyRepository.GetSingle(where, "Rooms");

                if (property.Rooms == null)
                    property.Rooms = new List<Room>();

                property.Rooms.Add(room);

                _unitOfWork.PropertyRepository.Edit(property);
                _unitOfWork.Save();

                return _mapper.Map<RoomOutput>(room);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Edit(string propertyId, string roomId, RoomInput newRoom)
        {
            try
            {

                Expression<Func<Property, bool>> where = p => p.Id == Guid.Parse(propertyId);

                Property property = _unitOfWork.PropertyRepository.GetSingle(where, "Rooms");

                List<Room> rooms = property.Rooms.ToList();

                Room old = rooms.Find(p => p.Id == Guid.Parse(roomId));
                property.UpdatedDate = old.UpdatedDate = DateTime.UtcNow;

                _mapper.Map(newRoom, old);

                _unitOfWork.PropertyRepository.Edit(property);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(string propertyId, string roomId)
        {
            try
            {
                Expression<Func<Property, bool>> where = p => p.Id == Guid.Parse(propertyId);

                Property property = _unitOfWork.PropertyRepository.GetSingle(where, "Rooms");

                List<Room> rooms = property.Rooms.ToList();

                rooms.Remove(rooms.Find(p => p.Id == Guid.Parse(roomId)));
                property.Rooms = rooms;
                property.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.PropertyRepository.Edit(property);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
