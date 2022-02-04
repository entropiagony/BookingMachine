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
    public class WorkPlaceRepository : IWorkPlaceRepository
    {
        private readonly DataContext db;

        public WorkPlaceRepository(DataContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<WorkPlace>> CreateWorkPlaces(int quantity, int floorId)
        {
            var workPlaces = new List<WorkPlace>();
            for (int i = 0; i < quantity; i++)
            {
                WorkPlace workPlace = new WorkPlace { FloorId = floorId };
                await db.WorkPlaces.AddAsync(workPlace);
                workPlaces.Add(workPlace);
            }
            return workPlaces;
        }

        public async Task DeleteWorkPlaceAsync(int id)
        {
            var workPlace = await db.WorkPlaces.FirstOrDefaultAsync(x => x.Id == id);
            if (workPlace == null)
                throw new NotFoundException("Specified work place doesn't exist");
            db.WorkPlaces.Remove(workPlace);
        }

        public async Task<WorkPlace> GetWorkPlaceAsync(int id)
        {
            return await db.WorkPlaces.FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}
