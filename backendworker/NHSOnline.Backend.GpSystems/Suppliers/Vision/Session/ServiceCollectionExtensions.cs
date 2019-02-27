using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionSessionServices(this IServiceCollection services)
        {
            services.AddTransient<VisionSessionService>();
            services.AddTransient<VisionSessionExtendService>();

            return services;
        }
    }
}