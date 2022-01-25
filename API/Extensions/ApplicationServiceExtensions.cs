using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using BusinessLogic.Utilities;
using Domain;
using Microsoft.EntityFrameworkCore;

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
            return services;
        }
    }
}

