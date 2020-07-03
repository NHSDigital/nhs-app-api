using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class AndroidDriverWrapper : IAndroidDriverWrapper
    {
        private readonly AndroidDriver<AndroidElement> _driver;
        private readonly Interactor<AndroidElement> _interactor;
        private readonly NativeDriverContext _nativeDriverContext;

        internal AndroidDriverWrapper(string testName, TestLogs logs)
        {
            Logs = logs;

            var browserStackConfig = Config.Get<BrowserStackConfig>("BrowserStack");
            var androidConfig = Config.Get<AndroidConfig>("Android");

            var options = new AppiumOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            browserStackConfig.SetCapabilities(options);
            androidConfig.SetCapabilities(options);

            options.AddAdditionalCapability("name", testName);

            options.AddAdditionalCapability("autoGrantPermissions", true);
            options.AddAdditionalCapability("nativeWebScreenshot", true);

            _driver = new AndroidDriver<AndroidElement>(new Uri("http://hub-cloud.browserstack.com/wd/hub"), options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _interactor = new Interactor<AndroidElement>(Logs, _driver.FindElement);
            _nativeDriverContext = new NativeDriverContext(_driver);
        }

        private TestLogs Logs { get; }

        public INativeWebContext Web() => new NativeWebInteractor(_nativeDriverContext, Logs, _driver);

        void IAndroidInteractor.ActOnElement(By @by, Action<AndroidElement> action, Action<InteractorOptions>? configure)
            => _interactor.ActOnElement(by, action, configure);

        void IDriverWrapper.AttachDebugInfo(IDriverCleanupContext context)
        {
            context.TryAttachScreenshot(_driver);
            context.TryAttachNativePageSource(_driver, _nativeDriverContext);
        }

        void IDriverWrapper.Cleanup(IDriverCleanupContext context)
        {
            context.TryCleanUp("quit Android driver", () => _driver?.Quit());
        }

        public void Dispose() => _driver.Dispose();
    }
}
