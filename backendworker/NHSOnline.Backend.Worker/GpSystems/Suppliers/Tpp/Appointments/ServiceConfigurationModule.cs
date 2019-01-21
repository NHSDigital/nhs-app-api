using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppAppointmentsService>();
            services.AddTransient<TppAppointmentSlotsService>();

            services.AddTransient<TppAppointmentsRetrievalService>();
            services.AddTransient<TppAppointmentsBookingService>();
            services.AddTransient<TppAppointmentsCancellationService>();

            services.AddTransient<IListSlotsReplyMapper, ListSlotsReplyMapper>();
            services.AddTransient<ISessionMapper, SessionMapper>();
            services.AddSingleton<IAppointmentSlotResultBuilder, TppAppointmentSlotsResultBuilder>();
            services.AddTransient<IAppointmentsReplyMapper, AppointmentsReplyMapper>();
            services.AddTransient<IAppointmentMapper, AppointmentMapper>();
            services.AddSingleton<IAppointmentsResultBuilder, TppAppointmentsResultBuilder>();

            base.ConfigureServices(services, configuration);
        }
    }
}
