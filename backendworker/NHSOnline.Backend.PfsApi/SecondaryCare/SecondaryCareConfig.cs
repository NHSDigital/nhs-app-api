using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareConfig : ISecondaryCareConfig
    {
        public Uri BaseUrl { get; }

        public SecondaryCareConfig(ILogger<SecondaryCareConfig> logger, IConfiguration configuration)
        {
            BaseUrl = new Uri(configuration.GetOrThrow("SECONDARY_CARE_BASE_URL", logger));
        }
    }
}