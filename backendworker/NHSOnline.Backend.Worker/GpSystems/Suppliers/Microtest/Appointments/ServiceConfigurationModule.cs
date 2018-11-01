using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<MicrotestAppointmentSlotsService>();
            services.AddTransient<MicrotestAppointmentsService>();

            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}
