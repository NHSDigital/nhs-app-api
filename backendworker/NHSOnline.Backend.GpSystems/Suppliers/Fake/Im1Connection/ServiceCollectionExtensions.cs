using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddTransient<FakeIm1ConnectionService>();
            services.AddTransient<DefaultIm1ConnectionBehaviour>();

            return services;
        }
    }
}