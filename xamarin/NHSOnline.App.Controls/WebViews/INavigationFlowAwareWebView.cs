using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.App.Controls.WebViews
{
    public interface INavigationFlowAwareWebView
    {
        void OnPageLoadComplete(WebViewPageNavigationEventArgs pageLoadEventArgs);
    }

    public class WebViewPageNavigationEventArgs : EventArgs
    {
        private readonly IReadOnlyCollection<(Uri, DateTimeOffset)> _pageNavigationUrlLog;

        public IEnumerable<(Uri, DateTimeOffset)> Urls => _pageNavigationUrlLog;

        public WebViewPageNavigationEventArgs(IEnumerable<(Uri, DateTimeOffset)> pageNavigationUrlLog)
        {
            _pageNavigationUrlLog = pageNavigationUrlLog.ToList().AsReadOnly();
        }
    }
}