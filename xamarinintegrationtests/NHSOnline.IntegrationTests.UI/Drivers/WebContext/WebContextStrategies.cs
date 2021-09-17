using System;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    public class WebContextStrategies
    {
        private readonly NativeDriverContext _nativeDriverContext;

        private readonly AppEvents _appEvents;
        private readonly NhsAppPreHomeWebViewContextStrategy _nhsAppPreHomeWebView;
        private readonly NhsAppWebViewContextStrategy _nhsAppLoggedInWebView;
        private readonly WebIntegrationWebViewContextStrategy _webIntegrationWebView;
        private readonly BrowserOverlayContextStrategy _browserOverlay;

        private readonly IWebDriver _driver;
        private readonly TestLogs _logs;

        internal WebContextStrategies(NativeDriverContext nativeDriverContext, IWebDriver driver, TestLogs logs)
        {
            _nativeDriverContext = nativeDriverContext;
            _driver = driver;
            _logs = logs;

            _appEvents = new AppEvents();

            _nhsAppPreHomeWebView = new NhsAppPreHomeWebViewContextStrategy(_appEvents, nativeDriverContext);
            _nhsAppLoggedInWebView = new NhsAppWebViewContextStrategy(_appEvents, nativeDriverContext, _nhsAppPreHomeWebView);

            _webIntegrationWebView = new WebIntegrationWebViewContextStrategy(
                _appEvents,
                nativeDriverContext,
                _nhsAppPreHomeWebView,
                _nhsAppLoggedInWebView);

            _browserOverlay = new BrowserOverlayContextStrategy(
                _appEvents,
                nativeDriverContext,
                _nhsAppPreHomeWebView,
                _nhsAppLoggedInWebView,
                _webIntegrationWebView);
        }

        public IWebInteractor NhsAppPreHomeWebView() => WrapStrategy(_nhsAppPreHomeWebView);
        public IWebInteractor NhsAppLoggedInWebView() => WrapStrategy(_nhsAppLoggedInWebView);
        public IWebInteractor WebIntegrationWebView() => WrapStrategy(_webIntegrationWebView);
        public IWebInteractor BrowserOverlayWebView() => WrapStrategy(_browserOverlay);

        private IWebInteractor WrapStrategy(IWebContextStrategy strategy)
        {
            return new NativeWebInteractor(_nativeDriverContext, _logs, _driver, strategy);
        }

        internal void AppClosed()
        {
            _appEvents.OnAppClosed();
        }

        public void NhsAppWebViewClosed()
        {
            _appEvents.OnNhsAppWebViewClosed();
        }
    }

    internal class AppEvents
    {
        internal event EventHandler? AppClosed;
        internal event EventHandler? NhsAppWebViewClosed;

        internal void OnAppClosed()
        {
            AppClosed?.Invoke(this, EventArgs.Empty);
        }

        internal void OnNhsAppWebViewClosed()
        {
            NhsAppWebViewClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}