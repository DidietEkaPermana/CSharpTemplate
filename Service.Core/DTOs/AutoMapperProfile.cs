using AutoMapper;
using Service.Core.Extensions;
using Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PropertyInput, Property>()
                .ForAllMembers(o => o.Condition((source, destination, member) => member != null));

            CreateMap<Property, PropertyOutput>();

            CreateMap<RoomInput, Room>()
                .ForMember(p => p.PropertyId, opt => opt.Ignore())
                .IgnoreZeroNumericProperties()
                .ForAllOtherMembers(o => o.Condition((source, destination, member) => member != null));

            CreateMap<Room, RoomOutput>();
        }
    }
}
