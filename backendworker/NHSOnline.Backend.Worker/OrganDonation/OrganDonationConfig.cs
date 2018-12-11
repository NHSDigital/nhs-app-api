using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public interface IOrganDonationConfig
    {
        Uri BaseUrl { get; set; }
        string ClientIdHeader { get; set; }
        string SubscriptionHeader { get; set; }
    }

    public class OrganDonationConfig : IOrganDonationConfig
    {
        public OrganDonationConfig(IConfiguration configuration, ILogger<OrganDonationConfig> logger)
        {
            BaseUrl = new Uri(configuration.GetOrThrow("ORGAN_DONATION_BASE_URL", logger));
            ClientIdHeader = configuration.GetOrThrow("ORGAN_DONATION_CLIENT_ID", logger);
            SubscriptionHeader = configuration.GetOrThrow("ORGAN_DONATION_OCP_APIM_SUBSCRIPTION_KEY", logger);
        }

        public Uri BaseUrl { get; set; }
        public string ClientIdHeader { get; set; }
        public string SubscriptionHeader { get; set; }
    }
}
