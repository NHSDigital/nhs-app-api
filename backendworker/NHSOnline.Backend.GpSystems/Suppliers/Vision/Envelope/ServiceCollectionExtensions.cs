using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionEnvelopeServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IEnvelopeService, EnvelopeService>()
                .AddSingleton<VisionPfsCertificate>()
                .AddTransient<IValidatable>(sp => sp.GetRequiredService<VisionPfsCertificate>());
        }
    }
}
