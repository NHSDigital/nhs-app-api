using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGpSystem, EmisGpSystem>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();

            services.AddTransient<EmisAppointmentsService>();
            services.AddTransient<EmisAppointmentSlotsService>();
            services.AddTransient<EmisCourseService>();
            services.AddTransient<EmisDemographicsService>();
            services.AddTransient<EmisIm1ConnectionService>();
            services.AddTransient<EmisPrescriptionService>();
            services.AddTransient<EmisSessionService>();
            services.AddTransient<EmisTokenValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}