using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestIm1ConnectionServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestIm1ConnectionService>();

            return services;
        }
    }
}