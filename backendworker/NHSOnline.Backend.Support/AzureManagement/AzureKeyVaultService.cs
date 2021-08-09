using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.AzureManagement
{
    public class AzureKeyVaultService: IAzureKeyVaultService
    {
        private readonly ILogger<AzureKeyVaultService> _logger;
        private readonly string _azureKeyVaultUri;
        private readonly string _cosmosDbRegion;

        public AzureKeyVaultService(
            IConfiguration configuration,
            ILogger<AzureKeyVaultService> logger)
        {
            _logger = logger;
            _azureKeyVaultUri = configuration.GetOrThrow("AZURE_KEY_VAULT_URI", _logger);
            _cosmosDbRegion = configuration.GetOrWarn("COSMOSDB_REGION", _logger);
        }

        public async Task<ConnectionStringResponse> GetConnectionStrings()
        {
            ConnectionStringResponse connectionStrings = null;
            try
            {
                var client = new SecretClient(new Uri(_azureKeyVaultUri),  new DefaultAzureCredential());

                _logger.LogInformation("Getting secrets");

                // Primary
                var getPrimaryConnectionStringResponse =
                    await client.GetSecretAsync("cosmosDBPrimaryConnectionString");
                var primaryConnectionStringSecret = getPrimaryConnectionStringResponse.Value;

                // Secondary
                var getSecondaryConnectionStringResponse =
                    await client.GetSecretAsync("cosmosDBSecondaryConnectionString");
                var secondaryConnectionStringSecret = getSecondaryConnectionStringResponse.Value;

                connectionStrings = new ConnectionStringResponse
                {
                    PrimaryConnectionString = $"{primaryConnectionStringSecret.Value}{_cosmosDbRegion}",
                    SecondaryConnectionString = $"{secondaryConnectionStringSecret.Value}{_cosmosDbRegion}",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not retrieve connection strings from KeyVault");
            }

            return connectionStrings;
        }
    }
}
