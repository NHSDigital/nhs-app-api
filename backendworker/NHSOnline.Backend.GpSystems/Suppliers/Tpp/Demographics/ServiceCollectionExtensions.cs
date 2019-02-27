using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppDemographicsServices(this IServiceCollection services)
        {
            services.AddTransient<TppDemographicsService>();
            services.AddTransient<ITppDemographicsMapper, TppDemographicsMapper>();

            return services;
        }
    }
}
