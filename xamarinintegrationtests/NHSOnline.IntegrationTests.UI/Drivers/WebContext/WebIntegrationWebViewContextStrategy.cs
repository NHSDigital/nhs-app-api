using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    internal class WebIntegrationWebViewContextStrategy : IWebContextStrategy
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private IWebContext? _webContext;

        public event EventHandler? SwitchedTo;

        public WebIntegrationWebViewContextStrategy(AppEvents appEvents, NativeDriverContext nativeDriverContext,
            NhsAppPreHomeWebViewContextStrategy preHomeWebViewContextStrategy,
            NhsAppWebViewContextStrategy nhsAppWebViewContextStrategy)
        {
            _nativeDriverContext = nativeDriverContext;

            appEvents.AppClosed += ResetWebContext;
            appEvents.LoggedOutHomeScreenLoaded += ResetWebContext;
            preHomeWebViewContextStrategy.SwitchedTo += ResetWebContext;
            nhsAppWebViewContextStrategy.SwitchedTo += ResetWebContext;
        }

        public void SwitchTo()
        {
            if (_webContext == null)
            {
                _nativeDriverContext.Logs.Info($"Grabbing context for a WebIntegration");
                _webContext = _nativeDriverContext.WebContextGrabber.GrabNextUnusedWebContext(WebContextKind.WebView);
                _nativeDriverContext.Logs.Info($"Grabbed {_webContext} for a WebIntegration");

                _nativeDriverContext.SwitchToWebContext(_webContext, AssertReady);
            }
            else
            {
                _nativeDriverContext.SwitchToWebContext(_webContext);
            }

            SwitchedTo?.Invoke(this, EventArgs.Empty);
        }

        private void AssertReady(IWebDriver driver)
        {
            driver.Url.Should().NotBe("about:blank");
        }

        private void ResetWebContext(object? sender, EventArgs e)
        {
            if (_webContext != null)
            {
                _webContext = null;
                _nativeDriverContext.Logs.Info("Reset WebIntegration context");
            }
        }
    }
}