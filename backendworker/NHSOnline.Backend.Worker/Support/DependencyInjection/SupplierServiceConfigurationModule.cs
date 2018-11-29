using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.DependencyInjection
{
    public abstract class SupplierServiceConfigurationModule : ServiceConfigurationModule
    {
        private readonly ILogger<ServiceConfigurationModule> _logger;
        protected abstract Supplier Supplier { get; } 
        
        protected SupplierServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }
 
        public static readonly IDictionary<Supplier, string> SupplierEnabledSettings =
            new Dictionary<Supplier, string>
            {
                { Supplier.Emis, "GP_PROVIDER_ENABLED_EMIS" },
                { Supplier.Tpp, "GP_PROVIDER_ENABLED_TPP" },
                { Supplier.Vision, "GP_PROVIDER_ENABLED_VISION" }
            };

        public override bool IsEnabled(IConfiguration configuration)
        {
            if (!SupplierEnabledSettings.TryGetValue(Supplier, out var settingName))
            {
                return false;
            }
                
            
            return bool.TryParse(configuration.GetOrWarn(settingName, _logger), out var enabled) &&
                   enabled;
        }
    }
}