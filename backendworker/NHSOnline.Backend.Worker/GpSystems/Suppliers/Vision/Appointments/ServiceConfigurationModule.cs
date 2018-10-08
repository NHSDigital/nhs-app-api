using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBookedAppointmentsResponseMapper, BookedAppointmentsResponseMapper>();
            services.AddTransient<IAppointmentMapper, AppointmentMapper>();
            services.AddTransient<ICancellationReasonMapper, CancellationReasonMapper>();
            services.AddTransient<VisionAppointmentsService>();
            services.AddTransient<VisionAppointmentsRetrievalService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}