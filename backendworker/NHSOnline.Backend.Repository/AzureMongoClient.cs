using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Repository
{
    public interface IAzureMongoClient
    {
        bool IsHealthy { get; set; }
        bool UsingPrimary { get; set; }
        INamedMongoClient PrimaryClient { get; set; }
        INamedMongoClient SecondaryClient { get; set; }
        INamedMongoClient ActiveClient { get; }

        void ReportAuthenticationFailure(AzureMongoClientType clientType);
        void Initialize(INamedMongoClient primary, INamedMongoClient secondary);
    }

    public class AzureMongoClient : IAzureMongoClient
    {
        private readonly ILogger<AzureMongoClient> _logger;

        public AzureMongoClient(ILogger<AzureMongoClient> logger)
        {
            _logger = logger;
        }

        public bool IsHealthy { get; set; }
        public bool UsingPrimary { get; set; }
        public INamedMongoClient PrimaryClient { get; set; }
        public INamedMongoClient SecondaryClient { get; set; }

        public void Initialize(INamedMongoClient primary, INamedMongoClient secondary)
        {
            IsHealthy = UsingPrimary = true;
            PrimaryClient = primary;
            SecondaryClient = secondary;
        }

        public INamedMongoClient ActiveClient => IsHealthy ? (UsingPrimary ? PrimaryClient : SecondaryClient) : null;

        public void ReportAuthenticationFailure(AzureMongoClientType clientType)
        {
            if (UsingPrimary && clientType == AzureMongoClientType.Primary)
            {
                _logger.LogInformation($"{nameof(ReportAuthenticationFailure)} - Switching to secondary client");
                UsingPrimary = false;
                return;
            }

            if (!UsingPrimary && clientType == AzureMongoClientType.Secondary)
            {
                _logger.LogInformation($"{nameof(ReportAuthenticationFailure)} - setting healthy to false");
                IsHealthy = false;
            }
        }
    }
}
