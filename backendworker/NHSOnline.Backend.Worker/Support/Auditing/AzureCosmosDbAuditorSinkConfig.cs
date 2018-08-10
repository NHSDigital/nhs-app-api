using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Auditing
{   
    public interface IAzureCosmosDbAuditorSinkConfig
    {
        Uri CosmosDbSinkUri { get; }
        string CosmosDbSinkKey { get; }
        string CosmosDbSinkDatabaseId { get; }
        string CosmosDbSinkCollectionId { get; }
    }
       
    public class AzureCosmosDbAuditorSinkConfig : IAzureCosmosDbAuditorSinkConfig
    {
        public Uri CosmosDbSinkUri { get; }
        public string CosmosDbSinkKey { get; }
        public string CosmosDbSinkDatabaseId { get; }
        public string CosmosDbSinkCollectionId { get; }

        public AzureCosmosDbAuditorSinkConfig(IConfiguration configuration, ILogger<AzureCosmosDbAuditorSinkConfig> logger)
        {
            var cosmosUriString = configuration.GetOrThrow("AUDIT_COSMOS_URI", logger);

            CosmosDbSinkUri = new Uri(cosmosUriString, UriKind.Absolute);
            CosmosDbSinkKey = configuration.GetOrThrow("AUDIT_COSMOS_KEY", logger);
            CosmosDbSinkDatabaseId = configuration.GetOrThrow("AUDIT_COSMOS_DATABASE_ID", logger);
            CosmosDbSinkCollectionId = configuration.GetOrThrow("AUDIT_COSMOS_COLLECTION_ID", logger);
        }
    }
}
