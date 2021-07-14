using System;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Keeping formating as consistent as possible with legacy app")]
    public sealed class UserAgentService : IUserAgentService
    {
        private static string Build(string name, string value) => $"nhsapp-{name}/{value}";
        private static string Platform => Build(DeviceInfo.Platform.ToString().ToLowerInvariant(), AppInfo.VersionString.ToLowerInvariant());
        private static string Manufacturer => Build("manufacturer", DeviceInfo.Manufacturer);
        private static string Model => Build("model", DeviceInfo.Model);
        private static string OperatingSystem => Build("os", DeviceInfo.VersionString.ToLowerInvariant());

        private static string Architecture =>
            Build("architecture", System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant());

        public static UserAgentService Instance { get; } = new UserAgentService();

        private UserAgentService()
        {
        }

        private static string SanitiseHeaderValue(string userAgent) =>
            userAgent.Replace(",", ".", StringComparison.Ordinal);

        public string NhsAppUserAgent =>
            SanitiseHeaderValue($"{Platform} {Manufacturer} {Model} {OperatingSystem} {Architecture}");
    }
}