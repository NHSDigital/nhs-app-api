using System;
using NHSOnline.App.Controls.WebViews;

namespace NHSOnline.App.DependencyServices
{
    public interface IPreHomeLogoutMonitor
    {
        void Begin();
        void PageLoadComplete(WebViewPageNavigationEventArgs pageNavigationEventArgs);
        void Finish(Uri? finalUrl);
    }
}