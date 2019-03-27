using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestDemographicsServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestDemographicsService>();
            services.AddTransient<IMicrotestDemographicsMapper, MicrotestDemographicsMapper>();

            return services;
        }
    }
}
