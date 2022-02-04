using Common.Exceptions;
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

        public async Task<Floor> CreateAsync(int floorNumber)
        {
            var floor = new Floor { FloorNumber = floorNumber };

            if (await db.Floors.AnyAsync(x => x.FloorNumber == floorNumber))
                throw new BadRequestException("This floor already exists");

            await db.Floors.AddAsync(floor);
            return floor;
        }

        public async Task DeleteFloorAsync(int id)
        {
            var floor = await db.Floors.FirstOrDefaultAsync(x => x.Id == id);
            if (floor == null)
                throw new NotFoundException("Specified floor doesn't exist");
            db.Floors.Remove(floor);
        }

        public async Task<Floor> GetFloorAsync(int id)
        {
            return await db.Floors.Include(x => x.WorkPlaces).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Floor>> GetFloorsAsync()
        {
            return await db.Floors.ToListAsync();
        }

        public async Task<IEnumerable<Floor>> GetFloorsWithWorkPlacesAsync()
        {
            return await db.Floors.Include(x => x.WorkPlaces).ToListAsync();
        }
    }
}
