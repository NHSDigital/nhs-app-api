using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    public class TermsAndConditionsConfig : ITermsAndConditionsConfig
    {
        public Uri EndpointUri { get; }
        public string AuthKey { get; }
        public string DatabaseId { get; }
        public string CollectionName { get; }
               
        public TermsAndConditionsConfig(IConfiguration configuration, ILogger<TermsAndConditionsConfig> logger)
        {
            var uriString = configuration.GetOrThrow("TERMS_CONDITIONS_COSMOS_ENDPOINT_URI", logger);
            EndpointUri = new Uri(uriString, UriKind.Absolute);
            AuthKey = configuration.GetOrThrow("TERMS_CONDITIONS_COSMOS_AUTH_KEY", logger);
            DatabaseId = configuration.GetOrThrow("TERMS_CONDITIONS_COSMOS_DATABASE_ID", logger);
            CollectionName = configuration.GetOrThrow("TERMS_CONDITIONS_COSMOS_COLLECTION_NAME", logger);
        }
    }
}