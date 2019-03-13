using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestConfig : ICertificateConfig
    {
        Uri BaseUrl { get; set; }
    }

    public class MicrotestConfig : IMicrotestConfig
    {
        public Uri BaseUrl { get; set; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }

        public MicrotestConfig(IConfiguration configuration, ILogger<MicrotestConfig> logger)
        {
            var baseUrlstring = configuration.GetOrWarn("MICROTEST_BASE_URL", logger);
            if (!string.IsNullOrEmpty(baseUrlstring))
            {
                BaseUrl = new Uri(baseUrlstring, UriKind.Absolute);
            }
            CertificatePath = configuration.GetOrWarn("MICROTEST_CERT_PATH", logger);
            CertificatePassphrase = configuration.GetOrWarn("MICROTEST_CERT_PASSPHRASE", logger);
        }
    }
}
