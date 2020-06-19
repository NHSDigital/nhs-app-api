using System;
using System.IO;
using System.Net;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class BrowserStackConfig
    {
        public string? User { get; set; } = "ops20";
        public string? Key { get; set; }
        public string? LocalIdentifier { get; set; } = $"int_test_{Dns.GetHostName()}";
        public string Project { get; set; } = "NHSApp";
        public string Build { get; set; } = $"{Dns.GetHostName()}-local";

        internal void SetCapabilities(AppiumOptions options)
        {
            options.AddAdditionalCapability("browserstack.user", User);
            options.AddAdditionalCapability("browserstack.key", GetKey());
            options.AddAdditionalCapability("browserstack.localIdentifier", LocalIdentifier);
            options.AddAdditionalCapability("project", Project);
            options.AddAdditionalCapability("build", Build);
            options.AddAdditionalCapability("browserstack.local", "true");
            options.AddAdditionalCapability("browserstack.debug", "true");
        }

        private string? GetKey()
        {
            if (Key != null)
            {
                return Key;
            }

            var keyFilePath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nhsonline",
                "secrets",
                "browserstack_accesskey");
            return File.ReadAllText(keyFilePath);
        }
    }
}