using API.Data.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Utilities
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<RegisterDto, AppUser>();
        }
    }
}
