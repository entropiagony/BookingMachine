using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly DataContext db;

        public FloorRepository(DataContext db)
        {
            this.db = db;
        }

        public async Task<Floor> GetFloorAsync(int id)
        {
            return await db.Floors.Include(x => x.WorkPlaces).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Floor>> GetFloorsAsync()
        {
            return await db.Floors.ToListAsync();
        }
    }
}
