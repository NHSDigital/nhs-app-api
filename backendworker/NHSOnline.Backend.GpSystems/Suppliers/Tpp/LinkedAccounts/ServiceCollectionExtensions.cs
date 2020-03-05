using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.LinkedAccounts;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppLinkedAccountsServices(this IServiceCollection services)
        {
            services.AddTransient<TppLinkedAccountsService>();

            return services;
        }
    }
}