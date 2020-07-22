using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidDriverWrapper : IAndroidDriverWrapper
    {
        private readonly AndroidDriver<AndroidElement> _driver;
        private readonly Interactor<AndroidDriver<AndroidElement>, AndroidElement> _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;

        internal AndroidDriverWrapper(string testName, TestLogs logs)
        {
            Logs = logs;

            _browserStackConfig = Config.Get<BrowserStackConfig>("BrowserStack");
            var androidConfig = Config.Get<AndroidConfig>("Android");

            var options = new AppiumOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            _browserStackConfig.SetCapabilities(options);
            androidConfig.SetCapabilities(options);

            options.AddAdditionalCapability("name", testName);

            options.AddAdditionalCapability("autoGrantPermissions", true);
            options.AddAdditionalCapability("nativeWebScreenshot", true);

            _driver = new AndroidDriver<AndroidElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _interactor = new Interactor<AndroidDriver<AndroidElement>, AndroidElement>(Logs, _driver, _driver.FindElement);
            _nativeDriverContext = new NativeDriverContext(_driver, WebViewLocatorStrategy.MultipleWindows(_driver));
        }

        private TestLogs Logs { get; }

        public IWebInteractor Web(WebViewContext webViewContext)
            => new NativeWebInteractor(_nativeDriverContext, Logs, _driver, webViewContext);

        void IInteractor<AndroidDriver<AndroidElement>, AndroidElement>.ActOnElementContext(
            By by,
            Action<ElementContext<AndroidDriver<AndroidElement>, AndroidElement>> action)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnElementContext(by, action);
        }

        void IAndroidInteractor.AssertElementDoesntExist(By @by)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.AssertElementDoesntExist(@by);
        }

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

        public void Dispose() => _driver.Dispose();
    }
}
