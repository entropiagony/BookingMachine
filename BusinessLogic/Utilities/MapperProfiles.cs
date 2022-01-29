using AutoMapper;
using BusinessLogic.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Utilities
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<RegisterDto, AppUser>();
            CreateMap<AppUser, ManagerDto>();
            CreateMap<UpdateUserDto, AppUser>();
            CreateMap<AppUser, UserInfoDto>().ForMember(dest => dest.ManagerName, opt => opt.
            MapFrom(src => src.Manager.FirstName + " " + src.Manager.LastName));
            CreateMap<BookingDto, Booking>().ForSourceMember(x => x.FloorId, opt => opt.DoNotValidate());
        }
    }
}
