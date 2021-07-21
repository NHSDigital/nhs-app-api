using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers.WebContext
{
    internal class NhsAppWebViewContextStrategy : IWebContextStrategy
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly NhsAppPreHomeWebViewContextStrategy _nhsAppPreHomeWebViewContextStrategy;
        private IWebContext? _webContext;

        internal event EventHandler? SwitchedTo;

        public NhsAppWebViewContextStrategy(NativeDriverContext nativeDriverContext,
            NhsAppPreHomeWebViewContextStrategy nhsAppPreHomeWebViewContextStrategy)
        {
            _nativeDriverContext = nativeDriverContext;
            _nhsAppPreHomeWebViewContextStrategy = nhsAppPreHomeWebViewContextStrategy;
        }

        public void SwitchTo()
        {
            if (_webContext == null)
            {
                _nhsAppPreHomeWebViewContextStrategy.EnsureContextGrabbed();

                _nativeDriverContext.Logs.Info("Grabbing context for NhsAppWebView");
                _webContext = _nativeDriverContext.WebContextGrabber.GrabNextUnusedWebContext(WebContextKind.WebView);
                _nativeDriverContext.Logs.Info($"Grabbed {_webContext} for NhsAppWebView");

                _nativeDriverContext.SwitchToWebContext(_webContext, AssertReady);
            }
            else
            {
                _nativeDriverContext.SwitchToWebContext(_webContext, null);
            }

            SwitchedTo?.Invoke(this, EventArgs.Empty);
        }

        private void AssertReady(IWebDriver driver)
        {
            Assert.IsTrue(driver.ExecuteJavaScript<bool>(
                    "return window.nhsAppPageLoadComplete === true;"),
                "window.nhsAppPageLoadComplete was not found to be true");

        }
    }
}