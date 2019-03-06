using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationConfig
    {
        Uri BaseUrl { get; set; }
        string ClientIdHeader { get; set; }
        int ReferenceDataExpirySeconds { get; set; }
        string SubscriptionHeader { get; set; }
    }

    public class OrganDonationConfig : IOrganDonationConfig
    {
        public OrganDonationConfig(IConfiguration configuration, ILogger<OrganDonationConfig> logger)
        {
            BaseUrl = new Uri(configuration.GetOrThrow("ORGAN_DONATION_BASE_URL", logger));
            ClientIdHeader = configuration.GetOrThrow("ORGAN_DONATION_CLIENT_ID", logger);
            SubscriptionHeader = configuration.GetOrThrow("ORGAN_DONATION_OCP_APIM_SUBSCRIPTION_KEY", logger);
            ReferenceDataExpirySeconds = int.Parse(
                configuration.GetOrThrow("ORGAN_DONATION_REFERENCE_DATA_EXPIRY_SECONDS", logger), 
                CultureInfo.InvariantCulture
            );
        }

        public Uri BaseUrl { get; set; }
        public string ClientIdHeader { get; set; }
        public int ReferenceDataExpirySeconds { get; set; }
        public string SubscriptionHeader { get; set; }
    }
}
