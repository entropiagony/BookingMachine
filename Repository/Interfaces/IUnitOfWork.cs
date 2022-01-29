using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUnitOfWork
    {
        UserManager<AppUser> UserManager { get; }
        SignInManager<AppUser> SignInManager { get; }
        IManagerRepository ManagerRepository { get; }
        IFloorRepository FloorRepository { get; }
        IWorkPlaceRepository WorkPlaceRepository { get; }
        IBookingRepository BookingRepository { get; }
        Task<bool> Complete();
        
    }
}
