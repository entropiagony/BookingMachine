using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Utilities;
using Domain.Entities;
using Xunit;

namespace BookingMachine.Tests
{
    public class MapperProfilesTests
    {
        [Fact]
        public void ValidAutomapperConfigurationTest_Success()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Booking, ManagerBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
                cfg.CreateMap<Booking, AdminBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
                cfg.CreateMap<Booking, EmployeeBookingDto>().ForMember(dest => dest.FloorNumber, opt => opt.
            MapFrom(src => src.Floor.FloorNumber));
                cfg.CreateMap<AppUser, UserInfoDto>().ForMember(dest => dest.ManagerName, opt => opt.
            MapFrom(src => src.Manager.FirstName + " " + src.Manager.LastName));
                cfg.CreateMap<AppUser, ManagerDto>();
            });
            configuration.AssertConfigurationIsValid();
        }
    }
}
