using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppSessionServices(this IServiceCollection services)
        {
            services.AddTransient<ITppSessionMapper, TppSessionMapper>();
            services.AddTransient<TppSessionService>();
            services.AddTransient<TppSessionExtendService>();

            services.AddTransient<ITppLogMessagingService, TppScopedLogMessagingService>();
            services.AddTransient<TppLogMessagingService>();

            return services;
        }
    }
}