using System;
using NHSOnline.IntegrationTests.UI.Drivers.Native;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    internal class BrowserOverlayContextStrategy : IWebContextStrategy
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private IWebContext? _webContext;

        public BrowserOverlayContextStrategy(
            AppEvents appEvents,
            NativeDriverContext nativeDriverContext,
            NhsAppPreHomeWebViewContextStrategy preHomeWebViewContextStrategy,
            NhsAppWebViewContextStrategy nhsAppWebViewContextStrategy,
            WebIntegrationWebViewContextStrategy webIntegrationWebViewContextStrategy)
        {
            _nativeDriverContext = nativeDriverContext;

            appEvents.AppClosed += ResetWebContext;
            appEvents.LoggedOutHomeScreenLoaded += ResetWebContext;
            preHomeWebViewContextStrategy.SwitchedTo += ResetWebContext;
            nhsAppWebViewContextStrategy.SwitchedTo += ResetWebContext;
            webIntegrationWebViewContextStrategy.SwitchedTo += ResetWebContext;
        }

        public void SwitchTo()
        {
            if (_webContext == null)
            {
                _nativeDriverContext.Logs.Info("Grabbing context for BrowserOverlay");
                _webContext = _nativeDriverContext.WebContextGrabber.GrabNextUnusedWebContext(WebContextKind.BrowserOverlay);
                _nativeDriverContext.Logs.Info($"Grabbed {_webContext} for BrowserOverlay");
            }

            _nativeDriverContext.SwitchToWebContext(_webContext);
        }

        private void ResetWebContext(object? sender, EventArgs e)
        {
            if (_webContext != null)
            {
                _webContext = null;
                _nativeDriverContext.Logs.Info("Reset BrowserOverlay context");
            }
        }
    }
}