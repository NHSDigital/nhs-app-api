using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class IOSDriverWrapper: IIOSDriverWrapper
    {
        private readonly IOSDriver<IOSElement> _driver;
        private readonly Interactor<IOSElement> _interactor;
        private readonly NativeDriverContext _nativeDriverContext;

        internal IOSDriverWrapper(string testName, TestLogs logs)
        {
            Logs = logs;

            var browserStackConfig = Config.Get<BrowserStackConfig>("BrowserStack");
            var iosConfig = Config.Get<IOSConfig>("iOS");

            var options = new AppiumOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            browserStackConfig.SetCapabilities(options);
            iosConfig.SetCapabilities(options);

            options.AddAdditionalCapability("name", testName);

            _driver = new IOSDriver<IOSElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _interactor = new Interactor<IOSElement>(Logs, _driver.FindElement);
            _nativeDriverContext = new NativeDriverContext(_driver);
        }

        private TestLogs Logs { get; }

        public INativeWebContext Web() => new NativeWebInteractor(_nativeDriverContext, Logs, _driver);

        void IIOSInteractor.ActOnElement(By @by, Action<IOSElement> action) => _interactor.ActOnElement(by, action);

        void IDriverWrapper.AttachDebugInfo(IDriverCleanupContext context)
        {
            context.TryAttachScreenshot(_driver);
            context.TryAttachNativePageSource(_driver, _nativeDriverContext);
        }

        void IDriverWrapper.Cleanup(IDriverCleanupContext context)
        {
            context.TryCleanUp("quit iOS driver", () => _driver?.Quit());
        }

        public void Dispose() => _driver.Dispose();
    }
}