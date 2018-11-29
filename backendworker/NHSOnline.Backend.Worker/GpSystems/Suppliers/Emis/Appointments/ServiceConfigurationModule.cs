using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Emis;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisAppointmentsService>();
            services.AddTransient<EmisAppointmentSlotsService>();

            services.AddTransient<EmisAppointmentsRetrievalService>();
            services.AddTransient<EmisAppointmentsBookingService>();
            services.AddTransient<EmisAppointmentsCancellationService>();
            
            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();
            services.AddTransient<IAppointmentSlotsMapper, AppointmentSlotsMapper>();
            services.AddTransient<IAppointmentsResponseMapper, AppointmentsResponseMapper>();
            services.AddTransient<IAppointmentsMapper, AppointmentsMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}
