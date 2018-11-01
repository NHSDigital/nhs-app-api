using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestConfig
    {
        Uri BaseUrl { get; set; }
    }

    public class MicrotestConfig : IMicrotestConfig
    {
        public Uri BaseUrl { get; set; }

        public MicrotestConfig(IConfiguration configuration, ILogger<MicrotestConfig> logger)
        {
            var baseUrlstring = configuration.GetOrWarn("MICROTEST_BASE_URL", logger);
            if (!string.IsNullOrEmpty(baseUrlstring))
            {
                BaseUrl = new Uri(baseUrlstring, UriKind.Absolute);
            }
        }
    }
}
