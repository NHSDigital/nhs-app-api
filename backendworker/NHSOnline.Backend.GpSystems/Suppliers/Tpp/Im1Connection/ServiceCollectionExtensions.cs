using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddSingleton<TppIm1RegisterErrorMapper>();
            services.AddTransient<TppIm1ConnectionService>();

            return services;
        }
    }
}