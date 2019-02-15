using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    public class TermsAndConditionsConfig : ITermsAndConditionsConfig
    {
        public Uri EndpointUri { get; }
        public string AuthKey { get; }
        public string DatabaseId { get; }
        public string CollectionName { get; }
        public bool Stubbed { get; }
               
        public TermsAndConditionsConfig(IConfiguration configuration, ILogger<TermsAndConditionsConfig> logger)
        {
            Stubbed = bool.TryParse(configuration.GetOrWarn("STUB_TERMS_AND_CONDITIONS", logger), out bool stubbed) && stubbed;

            if (Stubbed)
            {
                return;
            }
            
            var uriString = configuration.GetOrWarn("TERMS_CONDITIONS_COSMOS_ENDPOINT_URI", logger);
            EndpointUri = new Uri(uriString, UriKind.Absolute);
            AuthKey = configuration.GetOrWarn("TERMS_CONDITIONS_COSMOS_AUTH_KEY", logger);
            DatabaseId = configuration.GetOrWarn("TERMS_CONDITIONS_COSMOS_DATABASE_ID", logger);
            CollectionName = configuration.GetOrWarn("TERMS_CONDITIONS_COSMOS_COLLECTION_NAME", logger);
        }
    }
}
