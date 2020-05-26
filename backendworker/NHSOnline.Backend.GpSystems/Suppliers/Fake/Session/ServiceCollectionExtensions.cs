using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakePfsSessionServices(this IServiceCollection services)
        {
            services.AddTransient<FakeSessionService>();
            services.AddTransient<FakeSessionExtendService>();
            services.AddTransient<DefaultSessionBehaviour>();
            services.AddTransient<DefaultSessionExtendBehaviour>();

            return services;
        }

        public static IServiceCollection RegisterFakeCidSessionServices(this IServiceCollection services)
        {
            services.AddTransient<FakeSessionService>();
            services.AddTransient<DefaultSessionBehaviour>();
            return services;
        }
    }
}