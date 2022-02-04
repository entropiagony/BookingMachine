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
    public class WorkPlaceService : IWorkPlaceService
    {
        private readonly IUnitOfWork unitOfWork;

        public WorkPlaceService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<WorkPlace>> CreateWorkPlacesAsync(int quantity, int floorId)
        {
            if (await unitOfWork.FloorRepository.GetFloorAsync(floorId) == null)
                throw new NotFoundException("Specified floor doesn't exist");

            if (quantity <= 0)
                throw new BadRequestException("Quantity should be greater or equal to zero");

            var workplaces = await unitOfWork.WorkPlaceRepository.CreateWorkPlaces(quantity, floorId);

            if(await unitOfWork.Complete())
                return workplaces;

            throw new BadRequestException("Failed to create workplaces");
        }

        public async Task DeleteWorkplaceAsync(int id)
        {
            await unitOfWork.WorkPlaceRepository.DeleteWorkPlaceAsync(id);
            if (await unitOfWork.Complete())
                return;
            throw new BadRequestException("Failed to delete workplace");
        }
    }
}
