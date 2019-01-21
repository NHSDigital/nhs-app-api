using Microsoft.Extensions.DependencyInjection;
namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisLinkageServices(this IServiceCollection services)
        {
            services.AddTransient<EmisLinkageService>();
            services.AddTransient<IEmisLinkageMapper, EmisLinkageMapper>();
            services.AddSingleton<EmisLinkageValidationService>();

            return services;
        }
    }
}
