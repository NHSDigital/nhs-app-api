using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.IntegrationTests.UI.DeviceProperties
{
    public enum Platform
    {
        Ios,
        Android
    }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "The string being tested is lowercase")]
    public static class PlatformExtensions
    {
        public static string UserAgentDeviceTypePrefix(this Platform platform) => $"nhsapp-{platform.ToString().ToLowerInvariant()}";
    }
}