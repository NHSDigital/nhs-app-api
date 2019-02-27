using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPrescriptionsServices(this IServiceCollection services)
        {
            services.AddTransient<TppCourseService>();
            services.AddTransient<TppPrescriptionService>();
            services.AddTransient<TppPrescriptionRequestValidationService>();

            services.AddTransient<ITppCourseMapper, TppCourseMapper>();
            services.AddTransient<ITppPrescriptionMapper, TppPrescriptionMapper>();

            return services;
        }
    }
}
