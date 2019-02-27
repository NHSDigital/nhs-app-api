using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisSessionServices(this IServiceCollection services)
        {
            services.AddTransient<IEmisSessionService, EmisSessionService>();
            services.AddTransient<EmisSessionService>();
            services.AddTransient<EmisSessionExtendService>();

            return services;
        }
    }
}