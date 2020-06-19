using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class AndroidDriverWrapper: IAndroidDriverWrapper
    {
        private readonly AndroidDriver<AndroidElement> _driver;
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

            _nativeDriverContext = new NativeDriverContext(_driver);
        }

        private TestLogs Logs { get; }

        public INativeWebContext Web() => new WebInteractor(this);

        void IAndroidInteractor.ActOnElement(By @by, Action<AndroidElement> action)
        {
            var retryUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(2));
            while (true)
            {
                try
                {
                    var element = _driver.FindElement(by);
                    action(element);
                    return;
                }
                catch (StaleElementReferenceException e) when (DateTime.UtcNow < retryUntil)
                {
                    Logs.Info("{0}: Retrying", e.Message);
                }
                catch (NoSuchElementException e)
                {
                    Assert.Fail("No element found matching {0}\n{1}", by, e);
                }
            }
        }

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

        private sealed class WebInteractor : INativeWebContext
        {
            private readonly IWebDriver _driver;
            private readonly IDisposable _webContext;
            private readonly TestLogs _logs;

            public WebInteractor(AndroidDriverWrapper androidDriverWrapper)
            {
                _webContext = androidDriverWrapper._nativeDriverContext.Web();
                _driver = androidDriverWrapper._driver;
                _logs = androidDriverWrapper.Logs;
            }

            void IWebInteractor.ActOnElement(By @by, Action<IWebElement> action)
            {
                var retryUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(2));
                while (true)
                {
                    try
                    {
                        var element = _driver.FindElement(by);
                        action(element);
                        return;
                    }
                    catch (StaleElementReferenceException e) when (DateTime.UtcNow < retryUntil)
                    {
                        _logs.Info("{0}: Retrying", e.Message);
                    }
                }
            }

            public void Dispose() => _webContext.Dispose();
        }
    }
}
