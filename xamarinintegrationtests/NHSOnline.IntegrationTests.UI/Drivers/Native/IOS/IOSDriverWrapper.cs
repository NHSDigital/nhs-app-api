using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using NHSOnline.IntegrationTests.UI.Drivers.WebContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    // Valid combinations for device and OS for Browserstack App Automate can be found here
    // https://www.browserstack.com/list-of-browsers-and-platforms/app_automate
    internal sealed class IOSDriverWrapper: IIOSDriverWrapper
    {
        private readonly IIOSBrowserStackDriver _driver;
        private readonly IIOSInteractor _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;
        private readonly FlipbookGeneration _flipbookGeneration;

        private TestLogs Logs { get; }
        public WebContextStrategies Web { get; }

        public string AppVersionNumber { get; }
        private string TestName { get; }

        private IOSDevice Device { get; }
        private IOSVersion OsVersion { get; }

        internal IOSDriverWrapper(string testName,
            TestLogs logs,
            IOSBrowserStackCapability capabilities,
            IOSDevice device,
            IOSVersion osVersion)
        {
            Logs = logs;
            Device = device;
            OsVersion = osVersion;
            TestName = testName;
            Device = device;
            OsVersion = osVersion;


            _browserStackConfig = Configuration.Get<BrowserStackConfig>("BrowserStack");

            var iosConfig = Configuration.Get<IOSConfig>("iOS");
            var nhsAppConfig = Configuration.Get<NhsAppConfig>("NhsApp");
            var flipBookConfig = Configuration.Get<FlipBookConfig>("FlipBookConfig");

            _flipbookGeneration = new FlipbookGeneration(flipBookConfig.FlipBookPath, TestName);

            logs.TestDevice(device.ToName(), osVersion.ToName());

            var options = CreateAppiumOptions(iosConfig, testName, device, osVersion, capabilities);

            _driver = new IOSBrowserStackDriver(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);

            logs.BrowserStackSessionId(_driver.SessionId);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _nativeDriverContext = CreateNativeDriverContext(logs);
            _interactor = CreateInteractor();

            Web = new WebContextStrategies(_nativeDriverContext, _driver, logs);
            AppVersionNumber = nhsAppConfig.AppVersionNumber;

            AssertAppRunning();
        }

        private void AssertAppRunning()
        {
            _driver.GetAppState("com.nhs.online.dev.browserstack")
                .Should().Be(AppState.RunningInForeground, TestResultRetryExtensions.AppNotRunningMessage);
        }

        private IOSInteractor CreateInteractor()
        {
            return new IOSInteractor(
                _nativeDriverContext,
                new Interactor<IIOSBrowserStackDriver, IOSElement>(Logs, _driver, _driver.FindElement));
        }

        private NativeDriverContext CreateNativeDriverContext(TestLogs logs)
        {
            return new NativeDriverContext(
                _driver,
                _driver,
                new IOSWebViewLocatorStrategy(_driver),
                logs);
        }

        private AppiumOptions CreateAppiumOptions(
            IOSConfig iosConfig,
            string testName,
            IOSDevice targetDevice,
            IOSVersion osVersion,
            IOSBrowserStackCapability capabilities)
        {
            return _browserStackConfig.GetDefaultBuilder()
                .AddBrowserStackAppiumVersion(GetAppiumVersion(osVersion))
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

        // To use older operating systems the tests must use specific Appium versions with those operating systems these can be found using the BrowserStack tool here
        // https://www.browserstack.com/app-automate/capabilities
        private string GetAppiumVersion(IOSVersion iosVersion)
        {
            return iosVersion switch
            {
                IOSVersion.Eleven => "1.16.0",
                IOSVersion.Twelve => "1.18.0",
                IOSVersion.Thirteen => _browserStackConfig.DefaultAppiumVersion,
                _ => _browserStackConfig.DefaultAppiumVersion
            };
        }

        void IInteractor<IIOSBrowserStackDriver, IOSElement>.ActOnDriver(
            ActOnDriverAction<IIOSBrowserStackDriver, IOSElement> action)
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

        Task IIOSDriverWrapper.ResetNetwork() => ResetNetwork();

        async Task IIOSDriverWrapper.ResetNetworkAndWait(TimeSpan timeSpan)
        {
            await ResetNetwork();
            await Task.Delay(timeSpan);
        }

        private async Task ResetNetwork()
        {
            var browserStackApiClient = new BrowserStackApiClient(_browserStackConfig);
            await browserStackApiClient.ApplyNetworkProfile(_driver.SessionId, NetworkProfile.Reset);
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

        void IIOSDriverWrapper.CloseApp()
        {
            _driver.CloseApp();

            Web.AppClosed();
        }

        void IIOSDriverWrapper.LaunchApp()
        {
            _driver.LaunchApp();
        }

        public void AssertRunningInForeground()
        {
            AssertAppRunning();
        }

        void INativeDriverWrapper.NhsAppWebViewClosed()
        {
            Web.NhsAppWebViewClosed();
        }

        //Write test details to file for flip book generation
        void IDriverWrapper.WriteTestDetails(string parentJourney, string testName)
        {
            var flipBookTestDetails = new FlipbookTestDetails
            {
                AppVersion = AppVersionNumber,
                Device = $"iOS - {Device.ToName()}",
                OSVersion = OsVersion.ToName(),
                ParentJourney = parentJourney,
                TestName = $"{testName} - iOS",
                Folder = TestName
            };

            _flipbookGeneration.WriteTestDetails(flipBookTestDetails);
        }

        public void Screenshot(string screenshotName)
        {
            _flipbookGeneration.Screenshot(_driver, screenshotName);
        }

        public void Dispose() => _driver.Dispose();
    }
}