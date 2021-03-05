using System.Net;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidConfig
    {
        public string App { get; set; } = $"{Dns.GetHostName()}-android";
        public string Device { get; set; } = "Google Pixel 2";
        public string OperatingSystemVersion { get; set; } = "8.0";

        internal void SetCapabilities(AppiumOptions options)
        {
            options.AddAdditionalCapability("app", App);
            options.AddAdditionalCapability("device", Device);
            options.AddAdditionalCapability("os_version", OperatingSystemVersion);
            options.AddAdditionalCapability("disableAnimations", true);
        }
    }
}