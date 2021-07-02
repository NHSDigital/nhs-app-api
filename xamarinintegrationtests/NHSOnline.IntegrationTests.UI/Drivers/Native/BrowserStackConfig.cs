using System;
using System.IO;
using System.Net;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class BrowserStackConfig
    {
        public string? User { get; set; } = "ops20";
        public string? Key { get; set; }
        public string? LocalIdentifier { get; set; } = $"int_test_{Dns.GetHostName()}";
        public string AppiumVersion { get; set; } = "1.20.2";
        public string Project { get; set; } = "NHSApp";
        public string Build { get; set; } = $"{Dns.GetHostName()}-local";
        public bool EnableNetworkLogs { get; set; }

        internal AppiumOptionsBuilder GetDefaultBuilder()
        {
            var builder = new AppiumOptionsBuilder()
                .AddBrowserStackUser(User)
                .AddBrowserStackKey(GetKey())
                .AddBrowserStackLocalIdentifier(LocalIdentifier)
                .AddBrowserStackAppiumVersion(AppiumVersion)
                .AddProject(Project)
                .AddBuild(Build)
                .EnableBrowserStackLocal()
                .EnableBrowserStackDebug()
                .EnableBrowserStackAcceptInsecureCerts()
                .AddBrowserStackGpsLocation("40.730610,-73.935242");

            if (EnableNetworkLogs)
            {
                builder.EnableBrowserStackNetworkLogs();
            }

            return builder;
        }

        internal string? GetKey()
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