using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidDriverWrapper : IAndroidDriverWrapper
    {
        private readonly AndroidDriver<AndroidElement> _driver;
        private readonly IAndroidInteractor _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;

        internal AndroidDriverWrapper(string testName, TestLogs logs)
        {
            Logs = logs;

            _browserStackConfig = Configuration.Get<BrowserStackConfig>("BrowserStack");
            var androidConfig = Configuration.Get<AndroidConfig>("Android");

            var options = new AppiumOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            _browserStackConfig.SetCapabilities(options);
            androidConfig.SetCapabilities(options);
            logs.TestDevice(androidConfig.Device, androidConfig.OperatingSystemVersion);

            options.AddAdditionalCapability("name", testName);

            options.AddAdditionalCapability("autoGrantPermissions", true);
            options.AddAdditionalCapability("nativeWebScreenshot", true);
            options.AddAdditionalCapability("ensureWebviewsHavePages", true);

            _driver = new AndroidDriver<AndroidElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);
            logs.BrowserStackSessionId(_driver.SessionId);

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _nativeDriverContext = new NativeDriverContext(_driver, _driver, new AndroidWebViewLocatorStrategy(_driver));

            _interactor = new AndroidInteractor(
                _nativeDriverContext,
                new Interactor<AndroidDriver<AndroidElement>, AndroidElement>(Logs, _driver, _driver.FindElement));
        }

        private TestLogs Logs { get; }

        public IWebInteractor Web(WebViewContext webViewContext)
            => new NativeWebInteractor(_nativeDriverContext, Logs, _driver, webViewContext);

        void IInteractor<AndroidDriver<AndroidElement>, AndroidElement>.ActOnElementContext(
            By by,
            Action<ElementContext<AndroidDriver<AndroidElement>, AndroidElement>> action)
            => _interactor.ActOnElementContext(by, action);

        IAndroidInteractor IAndroidInteractor.CreateContainedInteractor(By findContainerBy)
            => _interactor.CreateContainedInteractor(findContainerBy);

        void IAndroidInteractor.PressTabKey()
            => _interactor.PressTabKey();

        void IAndroidInteractor.PressEnterKey()
            => _interactor.PressEnterKey();

        void IAndroidDriverWrapper.PressBackButton() =>_driver.PressKeyCode(AndroidKeyCode.Back);

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
