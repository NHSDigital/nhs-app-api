using System;
using System.Collections.ObjectModel;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class OpenWebIntegrationRequest
    {
        public OpenWebIntegrationRequest(Uri url, Collection<Uri> additionalDomains, Uri helpUrl)
        {
            Url = url;
            AdditionalDomains = additionalDomains;
            HelpUrl = helpUrl;
        }

        public Uri Url { get; set; }
        public Collection<Uri> AdditionalDomains { get; }
        public Uri HelpUrl { get; set; }
    }
}