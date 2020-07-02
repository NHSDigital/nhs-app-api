using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeDemographicsServices(this IServiceCollection services)
        {
            services.AddTransient<FakeDemographicsService>();

            return services;
        }
    }
}