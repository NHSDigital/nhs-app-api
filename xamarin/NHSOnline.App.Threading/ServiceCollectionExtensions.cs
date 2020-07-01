using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Threading
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThreadingServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IBackgroundExecutionService, BackgroundExecutionService>();
        }
    }
}
