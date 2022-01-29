using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext db;

        public UnitOfWork(UserManager<AppUser> userManager, DataContext db, SignInManager<AppUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            this.db = db;
        }



        public UserManager<AppUser> UserManager { get; }

        public SignInManager<AppUser> SignInManager { get; }
        public IManagerRepository ManagerRepository => new ManagerRepository(UserManager);

        public IFloorRepository FloorRepository => new FloorRepository(db);

        public IWorkPlaceRepository WorkPlaceRepository => new WorkPlaceRepository(db);
        public IBookingRepository BookingRepository => new BookingRepository(db);

        public async Task<bool> Complete()
        {
            return await db.SaveChangesAsync() > 0;
        }
    }
}
