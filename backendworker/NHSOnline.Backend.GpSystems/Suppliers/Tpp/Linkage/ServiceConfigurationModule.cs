using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppLinkageService>();
            services.AddSingleton<TppLinkageRequestValidationService>();
            services.AddTransient<ITppLinkageMapper, TppLinkageMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}