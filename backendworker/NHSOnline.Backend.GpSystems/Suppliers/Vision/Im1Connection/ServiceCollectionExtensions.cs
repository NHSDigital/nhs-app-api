using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddSingleton<VisionIm1RegisterErrorMapper>();
            services.AddTransient<VisionIm1ConnectionService>();

            return services;
        }
    }
}