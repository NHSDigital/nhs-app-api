using System;
using OpenQA.Selenium;

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
        {
            _nativeDriverContext = nativeDriverContext;
            _driver = driver;
            _webViewContext = webViewContext;
            _interactor = new Interactor<IWebDriver, IWebElement>(logs, _driver, driver.FindElement);
        }

        void IInteractor<IWebDriver, IWebElement>.ActOnElementContext(
            By by,
            Action<ElementContext<IWebDriver, IWebElement>> action)
        {
            _nativeDriverContext.SwitchToWebContext(_webViewContext);
            _webViewContext.AssertContextReady(_driver);
            _interactor.ActOnElementContext(by, action);
        }
    }
}