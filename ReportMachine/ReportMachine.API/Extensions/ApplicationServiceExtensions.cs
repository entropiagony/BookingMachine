using ReportMachine.BusinessLogic.Interfaces;
using ReportMachine.BusinessLogic.Services;
using ReportMachine.BusinessLogic.Utilities;
using ReportMachine.Domain;
using ReportMachine.Repository.Interfaces;
using ReportMachine.Repository.Repositories;

namespace ReportMachine.API.Extensions
{
    static public class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService<ConsumeRabbitMQHostedService>();
            services.AddSingleton<ReportContext>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IMessageHandlingService, MessageHandlingService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddAutoMapper(typeof(MapperProfiles).Assembly);
            return services;
        }
    }
}
