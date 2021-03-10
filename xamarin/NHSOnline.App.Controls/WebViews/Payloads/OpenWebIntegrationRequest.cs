using System;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class OpenWebIntegrationRequest
    {
        public OpenWebIntegrationRequest(Uri url)
        {
            Url = url;
        }

        public Uri Url { get; set; }
    }
}