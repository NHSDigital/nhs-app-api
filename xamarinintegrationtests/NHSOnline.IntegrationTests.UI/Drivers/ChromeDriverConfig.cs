using System.Collections.Generic;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class ChromeDriverConfig
    {
        public bool EnableVerboseLogging { get; set; }
        public bool SuppressInitialDiagnosticInformation { get; set; } = true;
        public Dictionary<string, bool> Arguments { get; set; } = new Dictionary<string, bool>();
    }
}
