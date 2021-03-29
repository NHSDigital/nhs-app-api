using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Config;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddFidoServices(this IServiceCollection services)
        {
            services
                .AddHttpClient<UafHttpClient>(ConfigureUafHttpClient);

            return services
                .AddTransient<IUafClient, UafClient>()
                .AddTransient<IFidoService, FidoService>()
                .AddTransient<FidoRegistrationService>();
        }

        private static void ConfigureUafHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            var configuration = serviceProvider.GetRequiredService<INhsLoginConfiguration>();
            httpClient.BaseAddress = configuration.UafBaseAddress;
        }
    }
}