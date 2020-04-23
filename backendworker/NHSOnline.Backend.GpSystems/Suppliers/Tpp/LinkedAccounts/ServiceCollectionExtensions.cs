using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppLinkedAccountsServices(this IServiceCollection services)
        {
            services.AddTransient<TppLinkedAccountsService>();
            services.AddTransient<ITppLinkedAccountsService, TppLinkedAccountsService>();

            return services;
        }
    }
}