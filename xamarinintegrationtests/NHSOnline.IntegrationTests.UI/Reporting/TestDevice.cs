namespace NHSOnline.IntegrationTests.UI.Reporting
{
    internal sealed class TestDevice
    {
        internal TestDevice(string name, string operatingSystemVersion)
        {
            Name = name;
            OperatingSystemVersion = operatingSystemVersion;
        }

        public string Name { get; }
        public string OperatingSystemVersion { get; }
    }
}