using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEnvelopeService, EnvelopeService>();
            base.ConfigureServices(services, configuration);
        }
    }
}
