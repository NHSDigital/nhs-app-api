using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionDemographicsServices(this IServiceCollection services)
        {
            services.AddTransient<VisionDemographicsService>();    
            services.AddTransient<IVisionDemographicsMapper, VisionDemographicsMapper>();

            return services;
        }
    }
}