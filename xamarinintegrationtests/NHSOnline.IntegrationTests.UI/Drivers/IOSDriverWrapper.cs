using System;
using System.IO;
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

        public void AttachDebugInfo(IDriverCleanupContext context)
        {
            TryAttachScreenshot(context);
            TryAttachPageSource(context);
        }

        public void Cleanup(IDriverCleanupContext context)
        {
            DestroyAndroidDriver(context);
        }

        private void TryAttachScreenshot(IDriverCleanupContext context)
        {
            if (_driver is ITakesScreenshot takesScreenShot)
            {
                context.TryAttach("screen shot", () =>
                {
                    var screenShot = takesScreenShot.GetScreenshot();
                    var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.png");
                    screenShot.SaveAsFile(fileName);
                    return fileName;
                });
            }
        }

        private void TryAttachPageSource(IDriverCleanupContext context)
        {
            context.TryAttach("app source", () =>
            {
                var pageSource = _driver.PageSource;
                var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.xml");
                File.WriteAllText(fileName, pageSource);
                return fileName;
            });

            context.TryAttach("page source", () =>
            {
                using var _ = _nativeDriverContext.Web();
                var pageSource = _driver.PageSource;
                var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.html");
                File.WriteAllText(fileName, pageSource);
                return fileName;
            });
        }

        private void DestroyAndroidDriver(IDriverCleanupContext context)
        {
            context.TryCleanUp("quit android driver", () => _driver?.Quit());
        }

        public void Dispose() => _driver.Dispose();
    }
}