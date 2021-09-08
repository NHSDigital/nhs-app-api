using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public sealed class OpenPostWebIntegrationRequest
    {
        public OpenPostWebIntegrationRequest(Uri url, Dictionary<string,string> postData, Collection<Uri> additionalDomains, Uri helpUrl)
        {
            Url = url;
            PostData = postData;
            AdditionalDomains = additionalDomains;
            HelpUrl = helpUrl;
        }

        public Uri Url { get; set; }
        public Dictionary<string, string> PostData { get; }
        public Collection<Uri> AdditionalDomains { get; }
        public Uri HelpUrl { get; set; }
    }
}