using System;
using System.Collections.Generic;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal abstract class WebViewLocatorStrategy
    {
        internal abstract void SwitchToWebView(IWebContext webContext);

        internal abstract IReadOnlyList<IWebContext> GetWebContexts(WebContextKind webContextKind);

        internal abstract void ForEachWebContext(Action<IWebContext> action);
    }
}
