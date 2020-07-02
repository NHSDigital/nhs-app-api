using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeLinkedAccountsServices(this IServiceCollection services)
        {
            services.AddTransient<FakeLinkedAccountsService>();

            return services;
        }
    }
}