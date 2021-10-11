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
        private readonly IReadOnlyCollection<Uri> _pageLoadUrlLog;

        public IEnumerable<Uri> Urls => _pageLoadUrlLog;

        public WebViewPageLoadEventArgs(IEnumerable<Uri> pageLoadUrlLog)
        {
            _pageLoadUrlLog = pageLoadUrlLog.ToList().AsReadOnly();
        }
    }
}