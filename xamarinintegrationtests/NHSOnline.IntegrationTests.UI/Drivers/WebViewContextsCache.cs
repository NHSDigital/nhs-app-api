using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class WebViewContextsCache
    {
        private readonly Dictionary<WebViewContext, string> _knownContexts = new Dictionary<WebViewContext, string>();

        private readonly List<string> _oneOffContexts = new List<string>();

        internal bool TryGet(WebViewContext webViewContext, [NotNullWhen(true)] out string? context)
        {
            if (webViewContext == WebViewContext.OneOff)
            {
                context = null;
                return false;
            }

            return _knownContexts.TryGetValue(webViewContext, out context);
        }

        internal void Add(WebViewContext webViewContext, string context)
        {
            if (webViewContext == WebViewContext.OneOff)
            {
                _oneOffContexts.Add(context);
            }
            else
            {
                _knownContexts[webViewContext] = context;
            }
        }

        internal IEnumerable<string> Contexts => _oneOffContexts.Concat(_knownContexts.Values);
    }
}