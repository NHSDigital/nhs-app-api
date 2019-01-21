using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppLinkageServices(this IServiceCollection services)
        {
            services.AddTransient<TppLinkageService>();
            services.AddSingleton<TppLinkageValidationService>();
            services.AddTransient<ITppLinkageMapper, TppLinkageMapper>();

            return services;
        }
    }
}