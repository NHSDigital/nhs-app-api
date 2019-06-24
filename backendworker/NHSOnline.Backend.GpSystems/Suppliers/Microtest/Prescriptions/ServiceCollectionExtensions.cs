using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestPrescriptionsServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestPrescriptionService>();
            services.AddTransient<MicrotestCourseService>();
            services.AddTransient<MicrotestPrescriptionValidationService>();
            services.AddTransient<IMicrotestPrescriptionMapper, MicrotestPrescriptionMapper>();

            return services;
        }
    }
}
