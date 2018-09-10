using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    [Serializable]
    public class TermsAndConditionsRecord
    {
        public TermsAndConditionsRecord(string nhsNumber, bool consentGiven, DateTimeOffset dateOfConsent)
        {
            Id = Guid.NewGuid().ToString();
            NhsNumber = nhsNumber;
            ConsentGiven = consentGiven;
            DateOfConsent = dateOfConsent;
        }
        
        [JsonProperty(PropertyName="Id")]
        public string Id { get; private set; }
        public string NhsNumber { get; }
        public bool ConsentGiven { get; }
        public DateTimeOffset DateOfConsent { get; }       
    }
}