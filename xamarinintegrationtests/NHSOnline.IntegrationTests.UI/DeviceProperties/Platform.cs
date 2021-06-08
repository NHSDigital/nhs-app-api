using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.IntegrationTests.UI.DeviceProperties
{
    public enum Platform
    {
        Ios,
        Android
    }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Keeping formatting as consistent as possible with legacy app")]
    public static class PlatformExtensions
    {
        public static string UserAgentDeviceTypePrefix(this Platform platform) => $"nhsapp-{platform.ToString().ToLowerInvariant()}";
    }
}