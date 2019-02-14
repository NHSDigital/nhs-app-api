using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Vision;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionLinkageService>();
            services.AddTransient<IVisionLinkageMapper, VisionLinkageMapper>();
            services.AddSingleton<VisionLinkageRequestValidationService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}
