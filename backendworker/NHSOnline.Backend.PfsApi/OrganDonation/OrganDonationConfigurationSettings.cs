using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{

    public class OrganDonationConfigurationSettings
    {
        public OrganDonationConfigurationSettings( Uri baseUrlString, string clientIdHeader, 
            string supscriptionHeader, int referenceDataExpirySeconds)
        {
            BaseUrl = baseUrlString; 
            ClientIdHeader = clientIdHeader;
            SubscriptionHeader = supscriptionHeader;
            ReferenceDataExpirySeconds = referenceDataExpirySeconds;
        }

        public Uri BaseUrl { get; set; }
        public string ClientIdHeader { get; set; }
        public int ReferenceDataExpirySeconds { get; set; }
        public string SubscriptionHeader { get; set; }
    }
}
