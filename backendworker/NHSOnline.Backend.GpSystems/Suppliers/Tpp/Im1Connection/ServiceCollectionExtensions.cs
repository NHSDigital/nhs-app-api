using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddTransient<TppIm1ConnectionService>();

            return services;
        }
    }
}