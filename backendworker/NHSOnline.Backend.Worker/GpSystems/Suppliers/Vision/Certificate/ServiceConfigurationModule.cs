using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<CertificateService>();
        }
    }
}