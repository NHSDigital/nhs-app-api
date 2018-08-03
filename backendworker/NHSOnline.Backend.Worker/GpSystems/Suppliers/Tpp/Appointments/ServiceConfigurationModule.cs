using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppAppointmentsService>();
            services.AddTransient<TppAppointmentSlotsService>();

            services.AddTransient<TppAppointmentsRetrievalService>();
            services.AddTransient<TppAppointmentsBookingService>();
            services.AddTransient<TppAppointmentsCancellationService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}
