using Microsoft.Extensions.DependencyInjection;


namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisAppointmentsServices(this IServiceCollection services)
        {
            services.AddTransient<EmisAppointmentsService>();
            services.AddTransient<EmisAppointmentSlotsService>();

            services.AddTransient<EmisAppointmentsRetrievalService>();
            services.AddTransient<EmisAppointmentsBookingService>();
            services.AddTransient<EmisAppointmentsCancellationService>();
            services.AddSingleton<EmisAppointmentsValidationService>();

            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();
            services.AddTransient<IAppointmentSlotsMapper, AppointmentSlotsMapper>();
            services.AddTransient<IAppointmentsResponseMapper, AppointmentsResponseMapper>();
            services.AddTransient<IAppointmentsMapper, AppointmentsMapper>();

            return services;
        }
    }
}
