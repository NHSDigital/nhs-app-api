using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class NhsApimConfig : INhsApimConfig
    {
        public Uri BaseUrl { get; }

        public string CertPath { get; }

        public string CertPassphrase { get; }

        public string Key { get; }

        public string Kid { get; }

        public NhsApimConfig(ILogger<SecondaryCareConfig> logger, IConfiguration configuration)
        {
            BaseUrl = new Uri(
                configuration.GetOrThrow("NHSAPP_APIM_BASE_URL", logger));

            CertPath = configuration.GetOrThrow("NHSAPP_APIM_PFX", logger);

            CertPassphrase = configuration.GetOrWarn("NHSAPP_APIM_PFX_PASSPHRASE", logger);

            Key = configuration.GetOrThrow("NHSAPP_APIM_KEY", logger);

            Kid = configuration.GetOrThrow("NHSAPP_APIM_KID", logger);
        }
    }
}