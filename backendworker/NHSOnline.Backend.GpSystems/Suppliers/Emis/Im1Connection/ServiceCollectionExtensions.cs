using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddTransient<EmisIm1ConnectionService>();

            return services;
        }
    }
}