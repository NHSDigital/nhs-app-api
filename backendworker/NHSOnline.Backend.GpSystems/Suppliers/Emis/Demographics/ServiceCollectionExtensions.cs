using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisDemographicsServices(this IServiceCollection services)
        {
            services.AddTransient<EmisDemographicsService>();
            services.AddTransient<IEmisDemographicsMapper, EmisDemographicsMapper>();

            return services;
        }
    }
}
