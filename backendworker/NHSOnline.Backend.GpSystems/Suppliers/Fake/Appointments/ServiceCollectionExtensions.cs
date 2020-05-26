using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeAppointmentsServices(this IServiceCollection services)
        {
            services.AddTransient<FakeAppointmentSlotsService>();
            services.AddTransient<FakeAppointmentsService>();
            services.AddTransient<FakeAppointmentsValidationService>();

            services.AddTransient<DefaultAppointmentsBehaviour>();
            services.AddTransient<DefaultAppointmentSlotsBehaviour>();

            return services;
        }
    }
}