using System;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models
{
    public class OlcDemographics
    {

        public string NhsNumber { get; set; }
        
        public string NameFull { get; set; }
        
        public Name Name { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
        
        public string AddressFull { get; set; }
    }
}