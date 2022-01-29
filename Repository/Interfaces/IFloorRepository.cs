using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IFloorRepository
    {
        public Task<IEnumerable<Floor>> GetFloorsAsync();
        public Task<Floor> GetFloorAsync(int id);

    }
}
