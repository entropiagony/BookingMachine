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
            CreateMap<CreateBookingDto, Booking>();
            CreateMap<Booking, EmployeeBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
            CreateMap<Booking, AdminBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
            CreateMap<Booking, ManagerBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
            CreateMap<Booking, ReportDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
        }
    }
}
