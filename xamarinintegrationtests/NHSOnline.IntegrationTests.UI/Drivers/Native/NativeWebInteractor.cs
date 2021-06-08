using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class NativeWebInteractor : IWebInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly IWebDriver _driver;
        private readonly WebViewContext _webViewContext;
        private readonly Interactor<IWebDriver, IWebElement> _interactor;

        public NativeWebInteractor(
            NativeDriverContext nativeDriverContext,
            TestLogs logs,
            IWebDriver driver,
            WebViewContext webViewContext)
            : this(nativeDriverContext, driver, webViewContext, new Interactor<IWebDriver, IWebElement>(logs, driver, driver.FindElement))
        {
        }

        private NativeWebInteractor(
            NativeDriverContext nativeDriverContext, IWebDriver driver,
            WebViewContext webViewContext,
            Interactor<IWebDriver, IWebElement> interactor)
        {
            _nativeDriverContext = nativeDriverContext;
            _driver = driver;
            _webViewContext = webViewContext;
            _interactor = interactor;
        }

        void IInteractor<IWebDriver, IWebElement>.ActOnDriver(
            ActOnDriverAction<IWebDriver, IWebElement> action)
        {
            ChangeContext();
            _interactor.ActOnDriver(action);
        }

        public IWebInteractor CreateContainedInteractor(By findBy)
                => new NativeWebInteractor(_nativeDriverContext, _driver, _webViewContext, _interactor.CreateContainedInteractor(findBy));

        public string GetUserAgent()
        {
            ChangeContext();
            return _driver.ExecuteJavaScript<string>("return window.navigator.userAgent;");
        }

        private void ChangeContext()
        {
            _nativeDriverContext.SwitchToWebContext(_webViewContext);
        }
    }
}