using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppConfig : ITppConfig
    {   
        public Uri ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationProviderId { get; set; }
        public string ApplicationDeviceType { get; set; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }

        public TppConfig(IConfiguration configuration, ILogger<TppConfig> logger)
        {           
            var apiUriString = configuration.GetOrWarn("TPP_BASE_URL", logger);
            if (!string.IsNullOrEmpty(apiUriString))
            {
                ApiUrl = new Uri(apiUriString, UriKind.Absolute);
            }
            
            ApiVersion = configuration.GetOrWarn("TPP_API_VERSION", logger);
            ApplicationName = configuration.GetOrWarn("TPP_APPLICATION_NAME", logger);
            ApplicationVersion = configuration.GetOrWarn("TPP_APPLICATION_VERSION", logger);
            ApplicationProviderId = configuration.GetOrWarn("TPP_APPLICATION_PROVIDER_ID", logger);
            ApplicationDeviceType = configuration.GetOrWarn("TPP_APPLICATION_DEVICE_TYPE", logger);
            CertificatePath = configuration.GetOrWarn("TPP_CERTIFICATE_PATH", logger);
            CertificatePassphrase = configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", logger);
        }
        
        public Guid CreateGuid()
        {
            return Guid.NewGuid();
        }
    }
}