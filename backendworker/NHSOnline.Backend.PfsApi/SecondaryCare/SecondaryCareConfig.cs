using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareConfig : ISecondaryCareConfig
    {
        public Uri BaseUrl { get; }
        public string EventsPath { get; }
        public string WaitTimesPath { get; }
        public bool WaitTimesEnabled { get; }

        public SecondaryCareConfig(ILogger<SecondaryCareConfig> logger, IConfiguration configuration)
        {
            BaseUrl = new Uri(configuration.GetOrThrow("SECONDARY_CARE_AGGREGATOR_BASE_URL", logger));
            EventsPath = configuration.GetOrThrow("SECONDARY_CARE_AGGREGATOR_EVENTS_PATH", logger);
            WaitTimesPath = configuration.GetOrThrow("SECONDARY_CARE_AGGREGATOR_WAIT_TIMES_PATH", logger);
            WaitTimesEnabled = bool.TrueString.Equals(configuration.GetOrThrow("SECONDARY_CARE_WAIT_TIMES_ENABLED", logger), StringComparison.OrdinalIgnoreCase);
        }
    }
}