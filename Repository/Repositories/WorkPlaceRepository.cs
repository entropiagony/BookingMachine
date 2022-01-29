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

        public async Task<WorkPlace> GetWorkPlaceAsync(int id)
        {
            return await db.WorkPlaces.FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}
