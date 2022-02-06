using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IWorkPlaceService
    {
        public Task<IEnumerable<WorkPlace>> CreateWorkPlacesAsync(int quantity, int floorId);
        public Task DeleteWorkPlaceAsync(int id);
    }
}
