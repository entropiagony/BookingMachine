using BusinessLogic.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FloorService : IFloorService
    {
        private readonly IUnitOfWork unitOfWork;

        public FloorService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Floor> CreateAsync(int floorNumber)
        {
            if (floorNumber <= 0)
                throw new BadRequestException("Floor number should be greater or equal to zero");
            var floor = await unitOfWork.FloorRepository.CreateAsync(floorNumber);

            if (await unitOfWork.Complete())
                return floor;

            throw new BadRequestException("Failed to create floor");
        }

        public async Task DeleteAsync(int id)
        {
            await unitOfWork.FloorRepository.DeleteFloorAsync(id);
            if (await unitOfWork.Complete())
                return;

            throw new BadRequestException("Failed to delete floor");
        }

        public async Task<IEnumerable<Floor>> GetFloorsAsync()
        {
            return await unitOfWork.FloorRepository.GetFloorsWithWorkPlacesAsync();
        }
    }
}
