using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionPrescriptionsServices(this IServiceCollection services)
        {
            services.AddTransient<VisionPrescriptionService>();
            services.AddTransient<VisionPrescriptionRequestValidationService>();
            services.AddTransient<IVisionPrescriptionMapper, VisionPrescriptionMapper>();
            services.AddTransient<VisionCourseService>();

            return services;
        }
    }
}
