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
            
            services.AddTransient<MicrotestAppointmentsBookingService>();

            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();

            return services;
        }
    }
}
