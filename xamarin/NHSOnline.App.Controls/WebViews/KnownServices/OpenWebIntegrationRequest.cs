using System;

namespace NHSOnline.App.Controls.WebViews.KnownServices
{
    public sealed class OpenWebIntegrationRequest
    {
        public OpenWebIntegrationRequest(Uri url, MenuTab menuTab)
        {
            Url = url;
            MenuTab = menuTab;
        }

        public Uri Url { get; set; }
        public MenuTab MenuTab { get; set; }
    }
}