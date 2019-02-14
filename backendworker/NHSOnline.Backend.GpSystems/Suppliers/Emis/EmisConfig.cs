using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public interface IEmisConfig : ICertificateConfig
    {
        Uri BaseUrl { get; set; }
        string ApplicationId { get; set; }
        string Version { get; set; }
    }

    public class EmisConfig : IEmisConfig
    {
        public Uri BaseUrl { get; set; }
        public string ApplicationId { get; set; }
        public string Version { get; set; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }

        public EmisConfig(IConfiguration configuration, ILogger<EmisConfig> logger)
        {
             var baseUrlstring = configuration.GetOrWarn("EMIS_BASE_URL", logger);
            if (!string.IsNullOrEmpty(baseUrlstring))
            {
                BaseUrl = new Uri(baseUrlstring, UriKind.Absolute);
            }
            ApplicationId = configuration.GetOrWarn("EMIS_APPLICATION_ID", logger);
            Version = configuration.GetOrWarn("EMIS_VERSION", logger);
            CertificatePath = configuration.GetOrWarn("EMIS_CERTIFICATE_PATH", logger);
            CertificatePassphrase = configuration.GetOrWarn("EMIS_CERTIFICATE_PASSWORD", logger);
        }
    }
}
