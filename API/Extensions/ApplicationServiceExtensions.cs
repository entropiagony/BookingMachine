using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Repositories;

namespace API.Extensions
{
    static public class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(MapperProfiles).Assembly);
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IWorkPlaceRepository, WorkPlaceRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();
            return services;
        }
    }
}

