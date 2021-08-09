using System;
using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI
{
    internal class AppiumOptionsBuilder
    {
        private readonly AppiumOptions _options = new AppiumOptions();

        public AppiumOptionsBuilder AddAcceptInsecureCertificates()
        {
            _options.AcceptInsecureCertificates = true;
            return this;
        }

        public AppiumOptionsBuilder AddPageLoadStrategy(PageLoadStrategy strategy)
        {
            _options.PageLoadStrategy = strategy;
            return this;
        }

        public AppiumOptionsBuilder AddProject(string project)
        {
            _options.AddAdditionalCapability("project", project);
            return this;
        }

        public AppiumOptionsBuilder AddBuild(string build)
        {
            _options.AddAdditionalCapability("build", build);
            return this;
        }

        public AppiumOptionsBuilder AddApp(string app)
        {
            _options.AddAdditionalCapability("app", app);
            return this;
        }

        public AppiumOptionsBuilder DisableAnimations()
        {
            _options.AddAdditionalCapability("disableAnimations", true);
            return this;
        }

        public AppiumOptionsBuilder AddDevice(string device)
        {
            _options.AddAdditionalCapability("device", device);
            return this;
        }

        public AppiumOptionsBuilder AddOsVersion(string osVersion)
        {
            _options.AddAdditionalCapability("os_version", osVersion);
            return this;
        }

        public AppiumOptionsBuilder AddTestName(string name)
        {
            _options.AddAdditionalCapability("name", name);
            return this;
        }

        public AppiumOptionsBuilder DisableAutoGrantPermissions()
        {
            _options.AddAdditionalCapability("autoGrantPermissions", false);
            return this;
        }

        public AppiumOptionsBuilder EnableNativeWebScreenshots()
        {
            _options.AddAdditionalCapability("nativeWebScreenshot", true);
            return this;
        }

        public AppiumOptionsBuilder EnableEnsureWebviewsHavePages()
        {
            _options.AddAdditionalCapability("ensureWebviewsHavePages", true);
            return this;
        }

        public AppiumOptionsBuilder EnableWebviewDetailsCollection()
        {
            _options.AddAdditionalCapability("enableWebviewDetailsCollection", true);
            return this;
        }

        public AppiumOptionsBuilder SetBrowserstackTimeout()
        {
            _options.AddAdditionalCapability("browserstack.idleTimeout", "240000");
            return this;
        }

        public AppiumOptionsBuilder EnableNativeWebTap()
        {
            _options.AddAdditionalCapability("nativeWebTap", true);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackUser(string? user)
        {
            _options.AddAdditionalCapability("browserstack.user", user);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackKey(string? key)
        {
            _options.AddAdditionalCapability("browserstack.key", key);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackLocalIdentifier(string? localIdentifier)
        {
            _options.AddAdditionalCapability("browserstack.localIdentifier", localIdentifier);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackAppiumVersion(string? appiumVersion)
        {
            _options.AddAdditionalCapability("browserstack.appium_version", appiumVersion);
            return this;
        }

        public AppiumOptionsBuilder EnableBrowserStackLocal()
        {
            _options.AddAdditionalCapability("browserstack.local", "true");
            return this;
        }

        public AppiumOptionsBuilder EnableBrowserStackDebug()
        {
            _options.AddAdditionalCapability("browserstack.debug", "true");
            return this;
        }

        public AppiumOptionsBuilder EnableBrowserStackNetworkLogs()
        {
            _options.AddAdditionalCapability("browserstack.networkLogs", true);
            return this;
        }

        public AppiumOptionsBuilder EnableBrowserStackAcceptInsecureCerts()
        {
            _options.AddAdditionalCapability("browserstack.acceptInsecureCerts", true);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackGpsLocation(string gpsLocation)
        {
            _options.AddAdditionalCapability("browserstack.gpsLocation", gpsLocation);
            return this;
        }

        public AppiumOptionsBuilder AddBrowserStackSignInToAppStore(Dictionary<string,string> appStoreCredentials)
        {
            _options.AddAdditionalCapability("browserstack.appStoreConfiguration", appStoreCredentials);
            return this;
        }

        public AppiumOptionsBuilder DisableBrowserStackNetwork()
        {
            _options.AddAdditionalCapability("browserstack.networkProfile", "no-network");
            return this;
        }

        public AppiumOptionsBuilder DisableAppDataClearing()
        {
            _options.AddAdditionalCapability("noReset", "true");
            _options.AddAdditionalCapability("fullReset", "false");
            return this;
        }

        public AppiumOptionsBuilder AddIOSBrowserStackCapability(IOSBrowserStackCapability capabilities)
        {
            switch (capabilities)
            {
                case IOSBrowserStackCapability.None:
                    return this;
                case IOSBrowserStackCapability.NoNetwork:
                    return DisableBrowserStackNetwork();
                case IOSBrowserStackCapability.ExtendedIdleTimeout:
                    return SetBrowserstackTimeout();
                default:
                    throw new ArgumentOutOfRangeException(nameof(capabilities), capabilities, null);
            }
        }

        public AppiumOptionsBuilder AddAndroidBrowserStackCapability(AndroidBrowserStackCapability capabilities, AndroidConfig androidConfig)
        {
            switch (capabilities)
            {
                case AndroidBrowserStackCapability.None:
                    return this;
                case AndroidBrowserStackCapability.SignInToGoogle:
                    return AddBrowserStackSignInToAppStore(androidConfig.GoogleCredentials());
                case AndroidBrowserStackCapability.NoNetwork:
                    return DisableBrowserStackNetwork();
                case AndroidBrowserStackCapability.ExtendedIdleTimeout:
                    return SetBrowserstackTimeout();
                default:
                    throw new ArgumentOutOfRangeException(nameof(capabilities), capabilities, null);
            }
        }

        public AppiumOptions Build()
        {
            return _options;
        }
    }
}