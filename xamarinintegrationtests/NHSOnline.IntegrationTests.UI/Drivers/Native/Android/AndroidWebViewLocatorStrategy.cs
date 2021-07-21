using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidWebViewLocatorStrategy : WebViewLocatorStrategy
    {
        private readonly IAndroidBrowserStackDriver _driver;

        public AndroidWebViewLocatorStrategy(IAndroidBrowserStackDriver driver)
        {
            _driver = driver;
        }

        internal override void SwitchToWebView(IWebContext webContext)
        {
            var androidWebContext = webContext.AssertPlatformWebContext<AndroidWebContext>();
            _driver.Context = androidWebContext.ContextName;
            _driver.SwitchTo().Window(androidWebContext.WindowHandle);
        }

        internal override IReadOnlyList<IWebContext> GetWebContexts(WebContextKind webContextKind)
        {
            var expectedIdentifier = webContextKind switch
            {
                WebContextKind.WebView => "webview_com",
                WebContextKind.BrowserOverlay => "webview_chrome",
                _ => throw new ArgumentOutOfRangeException(nameof(webContextKind), webContextKind, null)
            };

            var allContexts = _driver.Contexts;
            var context =
                allContexts.FirstOrDefault(x => x.Contains(expectedIdentifier, StringComparison.OrdinalIgnoreCase));

            if (context is null)
            {
                throw new AssertFailedException(
                    $"Web context for {webContextKind} not found. Contexts: {string.Join(", ", allContexts)}");
            }

            _driver.Context = context;

            return _driver.WindowHandles.Select(handle => new AndroidWebContext(context, handle))
                .ToList()
                .AsReadOnly();
        }

        internal override void ForEachWebContext(Action<IWebContext> action)
        {
            foreach (var context in _driver.Contexts.Where(context =>
                context.Contains("webview", StringComparison.OrdinalIgnoreCase)))
            {
                _driver.Context = context;

                foreach (var androidWebContext in _driver.WindowHandles.Select(handle => new AndroidWebContext(context, handle)))
                {
                    _driver.SwitchTo().Window(androidWebContext.WindowHandle);
                    action(androidWebContext);
                }
            }
        }

        private sealed class AndroidWebContext : IWebContext
        {
            private bool Equals(AndroidWebContext other)
            {
                return ContextName == other.ContextName && WindowHandle == other.WindowHandle;
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || obj is AndroidWebContext other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ContextName, WindowHandle);
            }

            internal string ContextName { get; }
            internal string WindowHandle { get; }

            public AndroidWebContext(string contextName, string windowHandle)
            {
                ContextName = contextName;
                WindowHandle = windowHandle;
            }

            public override string ToString()
            {
                return $"[{ContextName} - {WindowHandle}]";
            }
        }
    }
}