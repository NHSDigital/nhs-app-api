using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestAppointmentsServices(this IServiceCollection services)
        {
            services.AddSingleton<MicrotestAppointmentsValidationService>();
            services.AddTransient<MicrotestAppointmentSlotsService>();
            services.AddTransient<MicrotestAppointmentsService>();
            services.AddTransient<MicrotestAppointmentsRetrievalService>();
            services.AddTransient<MicrotestAppointmentsBookingService>();
            services.AddTransient<MicrotestAppointmentsCancellationService>();

            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();
            services.AddTransient<IAppointmentSlotsMapper, AppointmentSlotsMapper>();
            services.AddTransient<IAppointmentsResponseMapper, AppointmentsResponseMapper>();
            services.AddTransient<IAppointmentsMapper, AppointmentsMapper>();

            return services;
        }
    }
}
