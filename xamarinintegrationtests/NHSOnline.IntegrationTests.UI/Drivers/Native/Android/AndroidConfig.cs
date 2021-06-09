using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidConfig
    {
        public string App { get; set; } = $"{Dns.GetHostName()}-android";
        public string Device { get; set; } = "Google Pixel 3";
        public string OperatingSystemVersion { get; set; } = "9.0";

        public string? PlayStorePassword { get; set; }

        public string PlayStoreUser { get; set; } = "nhsappbrowserstack@gmail.com";

        internal void SetBaseCapabilities(AppiumOptions options)
        {
            options.AddAdditionalCapability("app", App);
            options.AddAdditionalCapability("device", Device);
            options.AddAdditionalCapability("os_version", OperatingSystemVersion);
            options.AddAdditionalCapability("disableAnimations", true);
        }

        internal void SetSignInToGoogleCapabilitiy(AppiumOptions options)
        {
            options.AddAdditionalCapability("browserstack.appStoreConfiguration", new Dictionary<string, string> {{ "username", PlayStoreUser },{ "password", GetUserPassword() }});
        }

        private string GetUserPassword()
        {
            if (PlayStorePassword != null)
            {
                return PlayStorePassword;
            }

            var keyFilePath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nhsonline",
                "secrets",
                "browserstack_playstore_user_pass");
            return File.ReadAllText(keyFilePath);
        }
    }
}