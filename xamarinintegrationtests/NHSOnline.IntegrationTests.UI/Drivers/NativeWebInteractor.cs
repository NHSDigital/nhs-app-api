using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class NativeWebInteractor : INativeWebContext
    {
        private readonly IDisposable _webContext;
        private readonly Interactor<IWebElement> _interactor;

        public NativeWebInteractor(NativeDriverContext nativeDriverContext, TestLogs logs, IWebDriver driver)
        {
            _webContext = nativeDriverContext.Web();
            _interactor = new Interactor<IWebElement>(logs, driver.FindElement);
        }

        void IWebInteractor.ActOnElement(By @by, Action<IWebElement> action, Action<InteractorOptions>? configure)
            => _interactor.ActOnElement(by, action, configure);

        public void Dispose() => _webContext.Dispose();
    }
}