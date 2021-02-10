using System.Net;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSConfig
    {
        public string? App { get; set; } = $"{Dns.GetHostName()}-ios";
        public string? Device { get; set; } = "iPhone 8";
        public string? OperatingSystemVersion { get; set; } = "13.0";

        internal void SetCapabilities(AppiumOptions options)
        {
            options.AddAdditionalCapability("app", App);
            options.AddAdditionalCapability("device", Device);
            options.AddAdditionalCapability("os_version", OperatingSystemVersion);
        }
    }
}