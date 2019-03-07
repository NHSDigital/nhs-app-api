using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class ServiceConfigurationModule: Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAppointmentSlotTypeScraper, AppointmentSlotTypeScraper>();
            base.ConfigureServices(services, configuration);
        }
    }
}