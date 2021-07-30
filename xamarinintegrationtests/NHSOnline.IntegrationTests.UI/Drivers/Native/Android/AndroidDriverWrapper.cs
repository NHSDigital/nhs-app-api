using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using NHSOnline.IntegrationTests.UI.Drivers.WebContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidDriverWrapper : IAndroidDriverWrapper
    {
        private readonly IAndroidBrowserStackDriver _driver;
        private readonly IAndroidInteractor _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;

        private TestLogs Logs { get; }
        public WebContextStrategies Web { get; }

        internal AndroidDriverWrapper(
            string testName,
            TestLogs logs,
            AndroidBrowserStackCapability capabilities,
            AndroidDevice targetDevice,
            AndroidOSVersion osVersion)
        {
            Logs = logs;

            _browserStackConfig = Configuration.Get<BrowserStackConfig>("BrowserStack");
            var androidConfig = Configuration.Get<AndroidConfig>("Android");

            logs.TestDevice(targetDevice.ToName(), osVersion.ToName());

            var options = CreateAppiumOptions(androidConfig, testName, capabilities, targetDevice, osVersion);

            _driver = new AndroidBrowserStackDriver(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options, logs);

            logs.BrowserStackSessionId(_driver.SessionId);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _nativeDriverContext = new NativeDriverContext(
                _driver,
                _driver,
                new AndroidWebViewLocatorStrategy(_driver),
                logs);

            Web = new WebContextStrategies(_nativeDriverContext, _driver, Logs);

            _interactor = new AndroidInteractor(
                _nativeDriverContext,
                new Interactor<IAndroidBrowserStackDriver, AndroidElement>(Logs, _driver, _driver.FindElement));

            RetrieveAppState().Should().Be(AppState.RunningInForeground, TestResultRetryExtensions.AppNotRunningMessage);
        }

        private AppiumOptions CreateAppiumOptions(
            AndroidConfig androidConfig,
            string testName,
            AndroidBrowserStackCapability capabilities,
            AndroidDevice targetDevice,
            AndroidOSVersion osVersion)
        {
            return _browserStackConfig.GetDefaultBuilder()
                .AddAcceptInsecureCertificates()
                .AddPageLoadStrategy(PageLoadStrategy.Normal)
                .AddApp(androidConfig.App)
                .DisableAnimations()
                .AddDevice(targetDevice.ToName())
                .AddOsVersion(osVersion.ToName())
                .AddTestName(testName)
                .DisableAutoGrantPermissions()
                .EnableNativeWebScreenshots()
                .EnableEnsureWebviewsHavePages()
                .AddAndroidBrowserStackCapability(capabilities, androidConfig)
                .Build();
        }

        void IInteractor<IAndroidBrowserStackDriver, AndroidElement>.ActOnDriver(
            ActOnDriverAction<IAndroidBrowserStackDriver, AndroidElement> action)
            => _interactor.ActOnDriver(action);

        IAndroidInteractor IAndroidInteractor.CreateContainedInteractor(By findContainerBy)
            => _interactor.CreateContainedInteractor(findContainerBy);

        void IAndroidInteractor.PressTabKey()
            => _interactor.PressTabKey();

        void IAndroidInteractor.PressEnterKey()
            => _interactor.PressEnterKey();

        void IAndroidInteractor.PressShiftTabKey() =>
            _interactor.PressShiftTabKey();

        void IAndroidInteractor.TouchScreenCentre()
            => _interactor.TouchScreenCentre();

        WaitForAction IAndroidDriverWrapper.PressBackButton()
        {
            _driver.PressKeyCode(AndroidKeyCode.Back);
            return new WaitForAction();
        }

        void IAndroidDriverWrapper.DismissKeyboard()
        {
            _driver.HideKeyboard();
        }

        void IAndroidDriverWrapper.SendKey(int key)
        {
            _driver.PressKeyCode(key);
        }

        void IAndroidDriverWrapper.PushTestFile()
        {
            _driver.PushFile("/sdcard/Download/NhsAppLogo.png",
                new FileInfo("../../../../NHSOnline.IntegrationTests.UI/Resources/NhsAppLogo.png"));
        }

        async Task IAndroidDriverWrapper.EnableAirplaneMode()
        {
            var browserStackApiClient = new BrowserStackApiClient(_browserStackConfig);
            await browserStackApiClient.ApplyNetworkProfile(_driver.SessionId, NetworkProfile.AirplaneMode);
        }

        public void CloseApp()
        {
            // To simulate how a user would close the app we first background it, wait a few moments and then close it.
            // If we don't do this then the Android Cookie persistence doesn't have chance to kick in and we lose fresh
            // cookies. This is a problem in the app, but usual user behaviour won't cause it to be symptomatic.
            _driver.BackgroundApp();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            _driver.CloseApp();

            Web.AppClosed();
        }

        public void LaunchApp() => _driver.LaunchApp();

        public void BackgroundApp() => _driver.BackgroundApp();

        public AndroidChromeApp OpenChromeApp()
        {
            _nativeDriverContext.SwitchToNativeContext();
            return AndroidChromeApp.Launch(_driver, _interactor);
         }

        void IAndroidDriverWrapper.AssertNotRunningInForeground()
            => RetrieveAppState().Should().NotBe(AppState.RunningInForeground);

        void IAndroidDriverWrapper.AssertRunningInForeground() => RetrieveAppState().Should().Be(AppState.RunningInForeground);

        private AppState RetrieveAppState() => _driver.GetAppState("com.nhs.online.nhsonline.browserstack");

        void IDriverWrapper.AttachDebugInfo(IDriverCleanupContext context)
        {
            context.TryAttachScreenshot(_driver);
            context.TryAttachNativePageSource(_driver, _nativeDriverContext);
        }

        void IDriverWrapper.Cleanup(IDriverCleanupContext context)
        {
            context.TryCleanUp("quit Android driver", () => _driver?.Quit());
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
