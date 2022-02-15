using AutoMapper;
using ReportMachine.BusinessLogic.DTOs;
using ReportMachine.Domain.Entities;

namespace ReportMachine.BusinessLogic.Utilities
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<ReportDto, Report>();
        }
    }
}
