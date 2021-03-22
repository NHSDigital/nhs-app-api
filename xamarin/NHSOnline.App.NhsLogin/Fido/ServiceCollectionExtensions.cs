using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddFidoServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IFidoService, FidoService>()
                .AddTransient<FidoRegistrationService>();
        }
    }
}