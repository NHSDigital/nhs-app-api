using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.NhsLogin
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNhsLoginServices(this IServiceCollection services)
        {
            return services
                .AddTransient<INhsLoginService, NhsLoginService>();
        }
    }
}