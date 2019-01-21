using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisPrescriptionsServices(this IServiceCollection services)
        {
            services.AddTransient<EmisCourseService>();
            services.AddTransient<EmisPrescriptionService>();
            services.AddTransient<EmisPrescriptionValidationService>();

            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();

            return services;
        }
    }
}
