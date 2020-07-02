using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeLinkageServices(this IServiceCollection services)
        {
            services.AddTransient<FakeLinkageService>();
            services.AddTransient<FakeLinkageValidationService>();

            return services;
        }
    }
}