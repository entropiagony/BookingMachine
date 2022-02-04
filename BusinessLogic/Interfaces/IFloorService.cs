using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IFloorService
    {
        public Task<IEnumerable<Floor>> GetFloorsAsync();
        public Task DeleteAsync(int id);
        public Task<Floor> CreateAsync(int floorNumber);
    }
}
