using API.Data;
using API.Interfaces;
using API.Services;
using API.Utilities;
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

            return services;
        }
    }
}

