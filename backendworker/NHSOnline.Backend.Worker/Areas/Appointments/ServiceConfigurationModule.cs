using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
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