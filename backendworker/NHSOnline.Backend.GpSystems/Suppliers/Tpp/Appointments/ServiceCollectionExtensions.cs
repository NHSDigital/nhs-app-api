using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppAppointmentsServices(this IServiceCollection services)
        {
            services.AddTransient<TppAppointmentsService>();
            services.AddTransient<TppAppointmentSlotsService>();

            services.AddTransient<TppAppointmentsRetrievalService>();
            services.AddTransient<TppAppointmentsBookingService>();
            services.AddTransient<TppAppointmentsCancellationService>();

            services.AddTransient<IAppointmentSlotsMapper, AppointmentSlotsMapper>();
            services.AddTransient<ISessionMapper, SessionMapper>();
            services.AddTransient<IAppointmentsReplyMapper, AppointmentsReplyMapper>();
            services.AddTransient<IAppointmentMapper, AppointmentMapper>();
            services.AddTransient<IAppointmentsResultBuilder, TppAppointmentsResultBuilder>();
            
            services.AddTransient<TppAppointmentsValidationService>();
            
            return services;
        }
    }
}
