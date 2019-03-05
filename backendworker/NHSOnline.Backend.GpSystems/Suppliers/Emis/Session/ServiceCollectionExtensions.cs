using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisPfsSessionServices(this IServiceCollection services)
        {
            services.AddTransient<IEmisSessionService, EmisSessionService>();
            services.AddTransient<EmisSessionExtendService>();

            return services;
        }

        public static IServiceCollection RegisterEmisCidSessionServices(this IServiceCollection services)
        {
            services.AddTransient<IEmisSessionService, EmisSessionService>();

            return services;
        }
    }
}