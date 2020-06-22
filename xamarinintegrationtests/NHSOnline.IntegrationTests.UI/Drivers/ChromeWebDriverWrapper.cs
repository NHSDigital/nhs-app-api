using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class ChromeWebDriverWrapper: IWebDriverWrapper
    {
        private readonly ChromeDriverService _chromeDriverService;
        private readonly IWebDriver _webDriver;
        private readonly Interactor<IWebElement> _interactor;

        internal ChromeWebDriverWrapper(TestLogs logs)
        {
            Logs = logs;
            var config = Config.Get<ChromeDriverConfig>("Chrome");

            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions
            {
                AcceptInsecureCertificates = true,
                PageLoadStrategy = PageLoadStrategy.Normal
            };
            options.AddArguments(config.Arguments.Where(kvp => kvp.Value).Select(kvp => kvp.Key));

            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeDriverService.LogPath = Path.Join(Path.GetTempPath(), "ChromeDriver.log");
            _chromeDriverService.EnableVerboseLogging = config.EnableVerboseLogging;
            _chromeDriverService.SuppressInitialDiagnosticInformation = config.SuppressInitialDiagnosticInformation;

            _webDriver = new ChromeDriver(_chromeDriverService, options);
            _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            _interactor = new Interactor<IWebElement>(Logs, _webDriver.FindElement);
        }

        private TestLogs Logs { get; }

        public void GoToUrl(Uri uri) => _webDriver.Navigate().GoToUrl(uri);

        void IWebInteractor.ActOnElement(By @by, Action<IWebElement> action) => _interactor.ActOnElement(by, action);

        public void AttachDebugInfo(IDriverCleanupContext context)
        {
            TryAttachScreenshot(context);
            TryAttachPageSource(context);
            TryAttachChromeDriverLog(context);
        }

        public void Cleanup(IDriverCleanupContext context)
        {
            DestroyChromeWebDriver(context);
        }

        private void TryAttachScreenshot(IDriverCleanupContext context)
        {
            if (_webDriver is ITakesScreenshot takesScreenShot)
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
            context.TryAttach("page source", () =>
            {
                var pageSource = _webDriver.PageSource;
                var fileName = Path.Join(Path.GetTempPath(), $"{context.TestName}.html");
                File.WriteAllText(fileName, pageSource);
                return fileName;
            });
        }

        private static void TryAttachChromeDriverLog(IDriverCleanupContext context)
            => context.TryAttach("chrome driver log", () => Path.Join(Path.GetTempPath(), "ChromeDriver.log"));

        private void DestroyChromeWebDriver(IDriverCleanupContext context)
        {
            context.TryCleanUp("close web driver", () => _webDriver?.Close());
            context.TryCleanUp("quit web driver", () => _webDriver?.Quit());
        }

        public void Dispose()
        {
            _webDriver.Dispose();
            _chromeDriverService.Dispose();
        }
    }
}