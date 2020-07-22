using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSDriverWrapper: IIOSDriverWrapper
    {
        private readonly IOSDriver<IOSElement> _driver;
        private readonly Interactor<IOSDriver<IOSElement>, IOSElement> _interactor;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly BrowserStackConfig _browserStackConfig;

        internal IOSDriverWrapper(string testName, TestLogs logs)
        {
            Logs = logs;

            _browserStackConfig = Config.Get<BrowserStackConfig>("BrowserStack");
            var iosConfig = Config.Get<IOSConfig>("iOS");

            var options = new AppiumOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            _browserStackConfig.SetCapabilities(options);
            iosConfig.SetCapabilities(options);

            options.AddAdditionalCapability("name", testName);

            _driver = new IOSDriver<IOSElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _interactor = new Interactor<IOSDriver<IOSElement>, IOSElement>(Logs, _driver, _driver.FindElement);
            _nativeDriverContext = new NativeDriverContext(_driver, WebViewLocatorStrategy.MultipleContexts(_driver));
        }

        private TestLogs Logs { get; }

        public IWebInteractor Web(WebViewContext webViewContext)
            => new NativeWebInteractor(_nativeDriverContext, Logs, _driver, webViewContext);
        void IInteractor<IOSDriver<IOSElement>, IOSElement>.ActOnElementContext(
            By by,
            Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action)

        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnElementContext(by, action);
        }

        void IIOSInteractor.AssertElementDoesntExist(By @by)
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
            context.TryCleanUp("quit iOS driver", () => _driver?.Quit());
        }

        void IDriverWrapper.UpdateBrowserStackStatusToFailed(IDriverCleanupContext context)
        {
            context.UpdateBrowserStackStatusToFailed(_driver, _browserStackConfig);
        }

        public void Dispose() => _driver.Dispose();
    }
}