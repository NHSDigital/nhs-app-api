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
        private Lazy<IWebContext> WebContext { get; }

        public event EventHandler? SwitchedTo;

        public NhsAppPreHomeWebViewContextStrategy(NativeDriverContext nativeDriverContext)
        {
            _nativeDriverContext = nativeDriverContext;

            WebContext = new Lazy<IWebContext>(valueFactory: () =>
            {
                _nativeDriverContext.Logs.Info("Grabbing context for PreHomeWebView");

                var context = _nativeDriverContext.WebContextGrabber.GrabNextUnusedWebContext(WebContextKind.WebView);

                _nativeDriverContext.Logs.Info($"Grabbed {context} for PreHomeWebView");

                return context;
            });
        }

        public void SwitchTo()
        {
            _nativeDriverContext.SwitchToWebContext(WebContext.Value, AssertReady);
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
            var _ = WebContext.Value;
        }
    }
}