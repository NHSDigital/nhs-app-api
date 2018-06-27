using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppConfig : ITppConfig
    {   
        public string ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationProviderId { get; set; }
        public string ApplicationDeviceType { get; set; }                

        public TppConfig(IConfiguration configuration, ILogger<TppConfig> logger)
        {           
            ApiUrl = configuration.GetOrWarn("TPP_BASE_URL", logger);
            ApiVersion = configuration.GetOrWarn("TPP_API_VERSION", logger);
            ApplicationName = configuration.GetOrWarn("TPP_APPLICATION_NAME", logger);
            ApplicationVersion = configuration.GetOrWarn("TPP_APPLICATION_VERSION", logger);
            ApplicationProviderId = configuration.GetOrWarn("TPP_APPLICATION_PROVIDER_ID", logger);
            ApplicationDeviceType = configuration.GetOrWarn("TPP_APPLICATION_DEVICE_TYPE", logger);
        }
        
        public Guid CreateGuid()
        {
            return Guid.NewGuid();
        }
    }
}