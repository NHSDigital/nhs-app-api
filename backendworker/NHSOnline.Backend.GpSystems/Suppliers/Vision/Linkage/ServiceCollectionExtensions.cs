using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionLinkageServices(this IServiceCollection services)
        {
            services.AddSingleton<VisionLinkagePostErrorMapper>();
            services.AddSingleton<VisionLinkageGetErrorMapper>();
            services.AddTransient<VisionLinkageService>();
            services.AddTransient<IVisionLinkageMapper, VisionLinkageMapper>();
            services.AddSingleton<VisionLinkageValidationService>();

            return services;
        }
    }
}
