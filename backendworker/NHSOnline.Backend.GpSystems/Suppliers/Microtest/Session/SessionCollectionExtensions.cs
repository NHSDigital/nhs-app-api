using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestSessionServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestSessionService>();

            return services;
        }
    }
}