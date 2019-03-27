using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestIm1ConnectionService>();

            return services;
        }
    }
}