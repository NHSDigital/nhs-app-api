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

            services.AddTransient<IListSlotsReplyMapper, ListSlotsReplyMapper>();
            services.AddTransient<ISessionMapper, SessionMapper>();
            services.AddSingleton<IAppointmentSlotResultBuilder, TppAppointmentSlotsResultBuilder>();
            services.AddTransient<IAppointmentsReplyMapper, AppointmentsReplyMapper>();
            services.AddTransient<IAppointmentMapper, AppointmentMapper>();
            services.AddSingleton<IAppointmentsResultBuilder, TppAppointmentsResultBuilder>();

            return services;
        }
    }
}
