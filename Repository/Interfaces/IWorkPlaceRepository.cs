using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IWorkPlaceRepository
    {
        public Task<WorkPlace> GetWorkPlaceAsync(int id);
        public Task<IEnumerable<WorkPlace>> CreateWorkPlaces(int quantity, int floorId);
        public Task DeleteWorkPlaceAsync(int id);
    }
}
