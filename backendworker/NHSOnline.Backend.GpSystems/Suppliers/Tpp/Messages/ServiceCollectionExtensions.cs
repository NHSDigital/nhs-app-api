using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Messages
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPatientMessagesServices(this IServiceCollection services)
        {
            services.AddTransient<TppPatientMessagesService>();
            services.AddTransient<ITppPatientMessagesMapper, TppPatientMessagesMapper>();

            return services;
        }
    }
}