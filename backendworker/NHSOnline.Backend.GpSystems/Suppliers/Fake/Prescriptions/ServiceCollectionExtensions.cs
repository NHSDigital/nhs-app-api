using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakePrescriptionServices(this IServiceCollection services)
        {
            services.AddTransient<FakeCourseService>();
            services.AddTransient<FakePrescriptionService>();
            services.AddTransient<FakePrescriptionValidationService>();
            services.AddTransient<DefaultCourseBehaviour>();
            services.AddTransient<DefaultPrescriptionBehaviour>();

            return services;
        }
    }
}