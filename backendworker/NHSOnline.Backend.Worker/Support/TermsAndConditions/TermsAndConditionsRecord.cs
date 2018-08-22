using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
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
        public string NhsNumber { get; private set; }
        public bool ConsentGiven { get; private set; }
        public DateTimeOffset DateOfConsent { get; private set; }       
    }
}