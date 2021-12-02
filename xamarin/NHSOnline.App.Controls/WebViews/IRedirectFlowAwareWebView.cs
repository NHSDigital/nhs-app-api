using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.App.Controls.WebViews
{
    public interface IRedirectFlowAwareWebView
    {
        void OnPageLoadComplete(WebViewPageLoadEventArgs pageLoadEventArgs);
    }

    public class WebViewPageLoadEventArgs : EventArgs
    {
        private readonly IReadOnlyCollection<(Uri, DateTimeOffset)> _pageLoadUrlLog;

        public IEnumerable<(Uri, DateTimeOffset)> Urls => _pageLoadUrlLog;

        public WebViewPageLoadEventArgs(IEnumerable<(Uri, DateTimeOffset)> pageLoadUrlLog)
        {
            _pageLoadUrlLog = pageLoadUrlLog.ToList().AsReadOnly();
        }
    }
}