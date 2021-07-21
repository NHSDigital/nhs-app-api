using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSWebViewLocatorStrategy : WebViewLocatorStrategy
    {
        private readonly IContextAware _driver;

        public IOSWebViewLocatorStrategy(IContextAware driver)
        {
            _driver = driver;
        }

        internal override void SwitchToWebView(IWebContext webContext)
        {
            var iosWebContext = webContext.AssertPlatformWebContext<IOSWebContext>();
            _driver.Context = iosWebContext.ContextName;
        }

        internal override IReadOnlyList<IWebContext> GetWebContexts(WebContextKind webContextKind)
        {
            return GetAllWebContexts().ToList().AsReadOnly();
        }

        internal override void ForEachWebContext(Action<IWebContext> action)
        {
            foreach (var context in GetAllWebContexts())
            {
                _driver.Context = context.ContextName;
                action(context);
            }
        }

        private IEnumerable<IOSWebContext> GetAllWebContexts()
        {
            return _driver.Contexts
                .Where(context => context.Contains("webview", StringComparison.OrdinalIgnoreCase))
                .Select(context => new IOSWebContext(context));
        }

        private sealed class IOSWebContext : IWebContext
        {
            private bool Equals(IOSWebContext other)
            {
                return ContextName == other.ContextName;
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || obj is IOSWebContext other && Equals(other);
            }

            public override int GetHashCode()
            {
                return ContextName.GetHashCode(StringComparison.Ordinal);
            }

            internal string ContextName { get; }

            public IOSWebContext(string contextName)
            {
                ContextName = contextName;
            }

            public override string ToString()
            {
                return $"[{ContextName}]";
            }
        }
    }
}