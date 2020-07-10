using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class NativeWebInteractor : INativeWebContext
    {
        private readonly IDisposable _webContext;
        private readonly Interactor<IWebElement> _interactor;

        public NativeWebInteractor(
            NativeDriverContext nativeDriverContext,
            TestLogs logs,
            IWebDriver driver,
            WebViewContext webViewContext)
        {
            _webContext = nativeDriverContext.Web(webViewContext);
            _interactor = new Interactor<IWebElement>(logs, driver.FindElement);
        }

        void IWebInteractor.ActOnElement(By @by, Action<IWebElement> action) => _interactor.ActOnElement(by, action);

        public void Dispose() => _webContext.Dispose();
    }
}