using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            return services
                .AddTransient(_ => DependencyService.Get<ICookies>());
        }
    }
}