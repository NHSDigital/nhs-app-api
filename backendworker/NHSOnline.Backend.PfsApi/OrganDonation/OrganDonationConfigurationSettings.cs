using System;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationConfigurationSettings
    {
        public OrganDonationConfigurationSettings( Uri baseUrlString, string clientIdHeader, 
            string subscriptionHeader, int referenceDataExpirySeconds)
        {
            BaseUrl = baseUrlString; 
            ClientIdHeader = clientIdHeader;
            SubscriptionHeader = subscriptionHeader;
            ReferenceDataExpirySeconds = referenceDataExpirySeconds;
        }

        public Uri BaseUrl { get; set; }
        public string ClientIdHeader { get; set; }
        public int ReferenceDataExpirySeconds { get; set; }
        public string SubscriptionHeader { get; set; }
    }
}
