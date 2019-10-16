using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.LinkedAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisLinkedAccountsServices(this IServiceCollection services)
        {
            services.AddTransient<EmisLinkedAccountsService>();

            return services;
        }
    }
}
