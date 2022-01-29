using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ManagerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ManagerDto>> GetManagers()
        {
            var managers = await unitOfWork.ManagerRepository.GetManagersAsync();
            var managerDtos = mapper.Map<IEnumerable<ManagerDto>>(managers);

            return managerDtos;
        }
    }
}
