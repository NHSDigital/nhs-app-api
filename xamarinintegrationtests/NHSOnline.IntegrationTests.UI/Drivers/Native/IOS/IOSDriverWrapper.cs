using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers.WebContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSDriverWrapper: IIOSDriverWrapper
    {
        private readonly IOSDriver<IOSElement> _driver;
        private readonly IIOSInteractor _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;

        private TestLogs Logs { get; }
        public WebContextStrategies Web { get; }

        internal IOSDriverWrapper(string testName,
            TestLogs logs,
            IOSBrowserStackCapability capabilities,
            IOSDevice device,
            IOSVersion osVersion)
        {
            Logs = logs;

            _browserStackConfig = Configuration.Get<BrowserStackConfig>("BrowserStack");
            var iosConfig = Configuration.Get<IOSConfig>("iOS");

            logs.TestDevice(device.ToName(), osVersion.ToName());

            var options = CreateAppiumOptions(iosConfig, testName, device, osVersion, capabilities);

            _driver = new IOSDriver<IOSElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);

            logs.BrowserStackSessionId(_driver.SessionId);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            DateTime
                .Parse(_driver.DeviceTime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
                .Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1), TestResultRetryExtensions.DeviceTimeSkewMessage);

            _nativeDriverContext = new NativeDriverContext(_driver, _driver, new IOSWebViewLocatorStrategy(_driver), logs);
            Web = new WebContextStrategies(_nativeDriverContext, _driver, logs);

            _interactor = new IOSInteractor(
                _driver,
                _nativeDriverContext,
                new Interactor<IOSDriver<IOSElement>, IOSElement>(Logs, _driver, _driver.FindElement));

            RetrieveAppState().Should().Be(AppState.RunningInForeground, TestResultRetryExtensions.AppNotRunningMessage);
        }

        private AppiumOptions CreateAppiumOptions(
            IOSConfig iosConfig,
            string testName,
            IOSDevice targetDevice,
            IOSVersion osVersion,
            IOSBrowserStackCapability capabilities)
        {
            return _browserStackConfig.GetDefaultBuilder()
                .AddAcceptInsecureCertificates()
                .AddPageLoadStrategy(PageLoadStrategy.Normal)
                .AddApp(iosConfig.App)
                .AddDevice(targetDevice.ToName())
                .AddOsVersion(osVersion.ToName())
                .EnableNativeWebTap()
                .AddTestName(testName)
                .AddIOSBrowserStackCapability(capabilities)
                .Build();
        }

        private AppState RetrieveAppState() => _driver.GetAppState("com.nhs.online.dev.browserstack");

        void IInteractor<IOSDriver<IOSElement>, IOSElement>.ActOnDriver(
            ActOnDriverAction<IOSDriver<IOSElement>, IOSElement> action)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver(action);
        }

        IIOSInteractor IIOSInteractor.CreateContainedInteractor(By findContainerBy) => _interactor.CreateContainedInteractor(findContainerBy);
        void IIOSInteractor.AssertElementCannotBeFound(By by, string because) => _interactor.AssertElementCannotBeFound(by, because);

        WaitForAction IIOSDriverWrapper.SwipeBack()
        {
            _driver.ExecuteScript("mobile: dragFromToForDuration", new Dictionary<string, string>
            {
                { "duration", "8" },
                { "fromX", "0" },
                { "fromY", "50" },
                { "toX", "151" },
                { "toY", "50" }
            });
            return new WaitForAction();
        }

        void IIOSDriverWrapper.PushTestFile() => _driver.PushFile("@com.google.chrome.ios:documents/test.txt",
            new FileInfo("../../../../NHSOnline.IntegrationTests.UI/Resources/test.txt"));

        async Task IIOSDriverWrapper.DisableNetwork()
        {
            var browserStackApiClient = new BrowserStackApiClient(_browserStackConfig);
            await browserStackApiClient.ApplyNetworkProfile(_driver.SessionId, NetworkProfile.NoNetwork);
        }

        void IDriverWrapper.AttachDebugInfo(IDriverCleanupContext context)
        {
            context.TryAttachScreenshot(_driver);
            context.TryAttachNativePageSource(_driver, _nativeDriverContext);
        }

        void IDriverWrapper.Cleanup(IDriverCleanupContext context)
        {
            context.TryCleanUp("quit iOS driver", () => _driver?.Quit());
        }

        void IDriverWrapper.UpdateBrowserStackStatusToFailed(IDriverCleanupContext context)
        {
            context.UpdateBrowserStackStatusToFailed(_driver, _browserStackConfig);
        }

        void IDriverWrapper.UpdateBrowserStackStatusToPassed(IDriverCleanupContext context)
        {
            context.UpdateBrowserStackStatusToPassed(_driver, _browserStackConfig);
        }

        void IDriverWrapper.AddBrowserStackSessionDetailsToLogs(
            IDriverCleanupContext context, TestLogs testLogs)
        {
            context.AddBrowserStackSessionDetailsToLogs(_driver, _browserStackConfig, testLogs);
        }

        public void Dispose() => _driver.Dispose();
    }
}