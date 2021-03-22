using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.NhsLogin.Fido;

namespace NHSOnline.App.NhsLogin
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNhsLoginServices(this IServiceCollection services)
        {
            return services
                .AddTransient<INhsLoginService, NhsLoginService>()
                .AddFidoServices();
        }
    }
}