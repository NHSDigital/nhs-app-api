using NHSOnline.Backend.Worker.Mocking.Emis;

namespace NHSOnline.Backend.Worker.Mocking
{
    public class MockingConfiguration
    {
        internal string WiremockBaseUrl { get; }
        internal EmisConfiguration EmisConfiguration { get; }

        public MockingConfiguration(string wiremockBaseUrl, EmisConfiguration emisConfiguration)
        {
            WiremockBaseUrl = wiremockBaseUrl;
            EmisConfiguration = emisConfiguration;
        }
    }
}
