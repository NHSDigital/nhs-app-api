using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton(IConfiguration.Configuration)
                .AddSingleton(IConfiguration.Configuration.NhsLogin)
                .AddSingleton(IConfiguration.Configuration.NhsAppApi)
                .AddSingleton(IConfiguration.Configuration.NhsAppWeb)
                .AddSingleton(IConfiguration.Configuration.BeforeYouStart);
        }
    }
}