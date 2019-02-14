using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Vision;
        
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