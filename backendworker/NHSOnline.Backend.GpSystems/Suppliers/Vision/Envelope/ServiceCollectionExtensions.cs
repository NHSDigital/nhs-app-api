using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionEnvelopeServices(this IServiceCollection services)
        {
            services.AddTransient<IEnvelopeService, EnvelopeService>();
            return services;
        }
    }
}
