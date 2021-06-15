using System;
using System.Collections.ObjectModel;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class OpenWebIntegrationRequest
    {
        public OpenWebIntegrationRequest(Uri url, Collection<Uri> additionalDomains)
        {
            Url = url;
            AdditionalDomains = additionalDomains;
        }

        public Uri Url { get; set; }
        public Collection<Uri> AdditionalDomains { get; }
    }
}