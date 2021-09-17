using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    internal class NhsAppPreHomeWebViewContextStrategy : IWebContextStrategy
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private IWebContext? _webContext;

        public event EventHandler? SwitchedTo;

        public NhsAppPreHomeWebViewContextStrategy(AppEvents appEvents, NativeDriverContext nativeDriverContext)
        {
            _nativeDriverContext = nativeDriverContext;

            appEvents.AppClosed += ResetWebContext;
            appEvents.NhsAppWebViewClosed += ResetWebContext;
        }

        public void SwitchTo()
        {
            if (_webContext == null)
            {
                var webContext = GrabWebContext();

                _nativeDriverContext.SwitchToWebContext(webContext, AssertReady);
            }
            else
            {
                _nativeDriverContext.SwitchToWebContext(_webContext);
            }


            SwitchedTo?.Invoke(this, EventArgs.Empty);
        }

        private void AssertReady(IWebDriver driver)
        {
            Assert.IsTrue(driver.ExecuteJavaScript<bool>(
                    "return window.nhsAppPageLoadComplete === true;"),
                "window.nhsAppPageLoadComplete was not found to be true");
        }

        internal void EnsureContextGrabbed()
        {
            var _ = GrabWebContext();
        }

        private IWebContext GrabWebContext()
        {
            if (_webContext == null)
            {
                _nativeDriverContext.Logs.Info("Grabbing context for PreHomeWebView");
                _webContext = _nativeDriverContext.WebContextGrabber.GrabNextUnusedWebContext(WebContextKind.WebView);
                _nativeDriverContext.Logs.Info($"Grabbed {_webContext} for PreHomeWebView");
            }

            return _webContext;
        }

        private void ResetWebContext(object? sender, EventArgs e)
        {
            if (_webContext != null)
            {
                _webContext = null;
                _nativeDriverContext.Logs.Info("Reset Pre-Home context");
            }
        }
    }
}