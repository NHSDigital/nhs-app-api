using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestLinkageServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestLinkageService>();
            services.AddSingleton<MicrotestLinkageRequestValidationService>();

            return services;
        }
    }
}
