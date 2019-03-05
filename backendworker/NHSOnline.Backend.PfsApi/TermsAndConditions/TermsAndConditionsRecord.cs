using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    [Serializable]
    public class TermsAndConditionsRecord
    {
        public TermsAndConditionsRecord(string nhsNumber, bool consentGiven, bool analyticsCookieAccepted, 
            DateTimeOffset dateOfConsent, DateTimeOffset? dateAnalyticsCookieAccepted)
        {
            Id = Guid.NewGuid().ToString();
            NhsNumber = nhsNumber;
            ConsentGiven = consentGiven;
            DateOfConsent = dateOfConsent;
            AnalyticsCookieAccepted = analyticsCookieAccepted;
            DateAnalyticsCookieAccepted = dateAnalyticsCookieAccepted;
        }
        
        [JsonProperty(PropertyName="id")]
        public string Id { get; private set; }
        public string NhsNumber { get; }
        public bool ConsentGiven { get; set; }
        public bool AnalyticsCookieAccepted { get; }
        public DateTimeOffset DateOfConsent { get; set; }
        public DateTimeOffset? DateAnalyticsCookieAccepted { get; }   
    }
}