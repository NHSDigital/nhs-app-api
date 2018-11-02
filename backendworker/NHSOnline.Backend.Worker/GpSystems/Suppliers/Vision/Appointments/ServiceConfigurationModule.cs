using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBookedAppointmentsResponseMapper, BookedAppointmentsResponseMapper>();
            services.AddTransient<IBookedAppointmentMapper, BookedAppointmentMapper>();
            services.AddTransient<IAvailableAppointmentsMapper, AvailableAppointmentsMapper>();
            services.AddTransient<IAvailableAppointmentsResponseMapper, AvailableAppointmentsResponseMapper>();
            services.AddTransient<VisionAppointmentSlotsService>();
            services.AddTransient<ICancellationReasonMapper, CancellationReasonMapper>();
            services.AddTransient<VisionAppointmentsService>();
            services.AddTransient<VisionAppointmentsBookingService>();
            services.AddTransient<VisionAppointmentsRetrievalService>();
            services.AddTransient<VisionAppointmentsCancellationService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}